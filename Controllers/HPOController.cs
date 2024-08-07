﻿using Microsoft.AspNetCore.Mvc;
using ClinicX.Data;
using ClinicX.ViewModels;
using ClinicX.Meta;

namespace ClinicX.Controllers
{
    public class HPOController : Controller
    {
        private readonly ClinicalContext _clinContext;
        private readonly HPOVM _hpo;
        private readonly IStaffUserData _staffUser;
        private readonly IConfiguration _config;        
        private readonly IHPOCodeData _hpoData;
        private readonly IClinicalNoteData _clinicaNoteData;        
        private readonly IMiscData _misc;
        private readonly ICRUD _crud;
        private readonly IAuditService _audit;

        public HPOController(ClinicalContext context, IConfiguration config)
        {
            _clinContext = context;
            _config = config;
            _staffUser = new StaffUserData(_clinContext);
            _hpo = new HPOVM();
            _hpoData = new HPOCodeData(_clinContext);
            _clinicaNoteData = new ClinicalNoteData(_clinContext);
            _crud = new CRUD(_config);
            _misc = new MiscData(_config);
            _audit = new AuditService(_config);
        }

        [HttpGet]
        public async Task<IActionResult> HPOTerm(int id, string? searchTerm)
        {
            try
            {
                string staffCode = _staffUser.GetStaffMemberDetails(User.Identity.Name).STAFF_CODE;
                _audit.CreateUsageAuditEntry(staffCode, "ClinicX - HPO", "ID=" + id.ToString());

                _hpo.clinicalNote = _clinicaNoteData.GetClinicalNoteDetails(id);
                _hpo.hpoTermDetails = _hpoData.GetExistingHPOTermsList(id);
                _hpo.hpoTerms = _hpoData.GetHPOTermsList();
                _hpo.hpoExtractVM = _hpoData.GetExtractedTermsList(id, _config);

                if(searchTerm != null) 
                { 
                    _hpo.hpoTerms = _hpo.hpoTerms.Where(t => t.Term.Contains(searchTerm)).ToList();
                    _hpo.searchTerm = searchTerm;
                }

                return View(_hpo);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { error = ex.Message, formName = "HPO" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddHPOTerm(int noteID, int termID)
        {
            try
            {                
                int success = _crud.CallStoredProcedure("Clinical Note", "Add HPO Term", noteID, termID, 0, "", "", "", "", User.Identity.Name);

                if (success == 0) { return RedirectToAction("ErrorHome", "Error", new { error = "Something went wrong with the database update.", formName = "HPO-add(SQL)" }); }

                return RedirectToAction("HPOTerm", new { id = noteID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { error = ex.Message, formName = "HPO-add" });
            }
        }
                
        [HttpPost]
        public async Task<IActionResult> AddHPOTermFromText(int termID, int noteID)
        {
            try
            {                
                int success = _crud.CallStoredProcedure("Clinical Note", "Add HPO Term", noteID, termID, 0, "", "", "", "", User.Identity.Name);

                if (success == 0) { return RedirectToAction("ErrorHome", "Error", new { error = "Something went wrong with the database update.", formName = "HPO-addFromText(SQL)" }); }

                return RedirectToAction("HPOTerm", new { id = noteID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { error = ex.Message, formName = "HPO-addFromText" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHPOTermFromNote(int id)
        {
            try
            {
                //int noteID = 0;
                int noteID = _misc.GetNoteIDFromHPOTerm(id);

                int success = _crud.CallStoredProcedure("Clinical Note", "Delete HPO Term", id, 0, 0, "", "", "", "", User.Identity.Name);

                if (success == 0) { return RedirectToAction("ErrorHome", "Error", new { error = "Something went wrong with the database update.", formName = "HPO-delete(SQL)" }); }

                return RedirectToAction("HPOTerm", new { id = noteID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { error = ex.Message, formName = "HPO-delete" });
            }
        }  
    }
}
