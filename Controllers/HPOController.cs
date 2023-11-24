﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ClinicX.Data;
using ClinicX.Models;
using ClinicX.ViewModels;
using System.Security.Cryptography;
using ClinicX.Meta;

namespace ClinicX.Controllers
{
    public class HPOController : Controller
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;

        public HPOController(ClinicalContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> HPOTerm(int id)
        {
            try
            {
                HPOVM hpo = new HPOVM();
                VMData vm = new VMData(_context);
                hpo.clinicalNote = vm.GetClinicalNoteDetails(id);
                hpo.hpoTermDetails = vm.GetExistingHPOTerms(id);
                hpo.hpoTerms = vm.GetHPOTerms();
                hpo.hpoExtractVM = vm.GetExtractedTerms(id, _config);

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
                CRUD crud = new CRUD(_config);
                crud.CallStoredProcedure("Clinical Note", "Add HPO Term", iNoteID, iTermID, 0, "", "", "", "", User.Identity.Name);

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
                CRUD crud = new CRUD(_config);
                crud.CallStoredProcedure("Clinical Note", "Add HPO Term", iNoteID, iTermID, 0, "", "", "", "", User.Identity.Name);

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
                int iNoteID = 0;

                CRUD crud = new CRUD(_config);
                crud.CallStoredProcedure("Clinical Note", "Delete HPO Term", iID, 0, 0, "", "", "", "", User.Identity.Name);

                iNoteID = GetNoteID(iID);

                return RedirectToAction("HPOTerm", new { id = iNoteID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorHome", "Error", new { sError = ex.Message });
            }
        }     
        
        private int GetNoteID(int iID)
        {
            try
            {
                SqlConnection conn = new SqlConnection(_config.GetConnectionString("ConString"));
                conn.Open();

                SqlCommand cmd1 = new SqlCommand("select ClinicalNoteID from ClinicalNotesHPOTerm where ID=" + iID, conn);

                int iNoteID = 0;
                using (var reader = cmd1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        iNoteID = reader.GetInt32(0);
                    }
                }
                conn.Close();
                return iNoteID;
            }
            catch (Exception ex)
            {
                RedirectToAction("ErrorHome", "Error", new { sError = ex.Message });
                return 0;
            }
        }

    }
}
