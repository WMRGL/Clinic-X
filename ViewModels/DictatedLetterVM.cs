﻿using ClinicalXPDataConnections.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicX.ViewModels
{
    [Keyless]
    public class DictatedLetterVM
    {
        public StaffMember staffUser { get; set; }
        public List<DictatedLettersCopy> dictatedLettersCopies  { get; set; }
        public List<Patient> dictatedLettersPatients { get; set; }
        public DictatedLetter dictatedLetters { get; set; }
        public List<DictatedLetter> dictatedLettersForApproval { get; set; }
        public List<DictatedLetter> dictatedLettersForPrinting { get; set; }
        public List<Patient> patients { get; set; }
        public List<StaffMember> staffMemberList { get; set;}
        public Patient patientDetails { get; set; }        
        public List<Patient> patientList { get; set; }
        public ActivityItem activityDetails { get; set; }
        public ExternalFacility referrerFacility { get; set; }
        public ExternalClinician referrer { get; set; }
        public ExternalFacility GPFacility { get; set; }
        public List<ExternalFacility> facilities { get; set; }
        public List<ExternalCliniciansAndFacilities> clinicians { get; set; }        
        public List<StaffMember> consultants { get; set; }
        public List<StaffMember> gcs { get; set; }
        public List<string> secteams { get; set; }
        public List<string> specialities { get; set; }
        public List<ActivityItem> activities { get; set; }
    }
}
