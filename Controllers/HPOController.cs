﻿using Microsoft.AspNetCore.Mvc;
using ClinicX.Data;
using ClinicX.ViewModels;
using ClinicX.Meta;

namespace ClinicX.Controllers
{
    public class HPOController : Controller
    {
        private readonly ClinicalContext _clinContext;
        private readonly IConfiguration _config;
        private readonly HPOVM hpo;
        private readonly VMData vm;
        private readonly CRUD crud;
        private readonly MiscData misc;

        public HPOController(ClinicalContext context, IConfiguration config)
        {
            _clinContext = context;
            _config = config;
            hpo = new HPOVM();
            vm = new VMData(_clinContext);
            crud = new CRUD(_config);
            misc = new MiscData(_config);
        }

        [HttpGet]
        public async Task<IActionResult> HPOTerm(int id, string? sSearchTerm)
        {
            try
            {                
                hpo.clinicalNote = vm.GetClinicalNoteDetails(id);
                hpo.hpoTermDetails = vm.GetExistingHPOTermsList(id);
                hpo.hpoTerms = vm.GetHPOTermsList();
                hpo.hpoExtractVM = vm.GetExtractedTermsList(id, _config);

                if(sSearchTerm != null) 
                { 
                    hpo.hpoTerms = hpo.hpoTerms.Where(t => t.Term.Contains(sSearchTerm)).ToList();
                }

                return View(hpo);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { sError = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddHPOTerm(int iNoteID, int iTermID)
        {
            try
            {                
                int iSuccess = crud.CallStoredProcedure("Clinical Note", "Add HPO Term", iNoteID, iTermID, 0, "", "", "", "", User.Identity.Name);

                if (iSuccess == 0) { return RedirectToAction("Index", "WIP"); }

                return RedirectToAction("HPOTerm", new { id = iNoteID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { sError = ex.Message });
            }
        }
                
        [HttpPost]
        public async Task<IActionResult> AddHPOTermFromText(int iTermID, int iNoteID)
        {
            try
            {                
                int iSuccess = crud.CallStoredProcedure("Clinical Note", "Add HPO Term", iNoteID, iTermID, 0, "", "", "", "", User.Identity.Name);

                if (iSuccess == 0) { return RedirectToAction("Index", "WIP"); }

                return RedirectToAction("HPOTerm", new { id = iNoteID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { sError = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHPOTermFromNote(int iID)
        {
            try
            {
                //int iNoteID = 0;
                int iNoteID = misc.GetNoteIDFromHPOTerm(iID);

                int iSuccess = crud.CallStoredProcedure("Clinical Note", "Delete HPO Term", iID, 0, 0, "", "", "", "", User.Identity.Name);

                if (iSuccess == 0) { return RedirectToAction("Index", "WIP"); }

                return RedirectToAction("HPOTerm", new { id = iNoteID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { sError = ex.Message });
            }
        }  
    }
}
