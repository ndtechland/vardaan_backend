using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;

namespace VardaanCab.Repository
{
    public class StateMasterGstinRepository
    {
        DbEntities ent = new DbEntities();

       

        public IEnumerable<StateWiseGstinDTO> GetStateMasterGstinList()
        {
            var states = (from s in ent.StateMasters
                          join sg in ent.StateWiseGSTINs
on s.Id equals sg.State_Id
                          where sg.IsActive
                          select new StateWiseGstinDTO
                          {
                              Id = sg.Id,
                              StateName = s.StateName,
                              Address = sg.Address,
                              Gstin = sg.Gstin,
                              BankName=sg.BankName,
                              BranchAddress=sg.BranchAddress,
                              AC_No=sg.AC_No,
                              IFS_Code=sg.IFS_Code,
                              IsActive = sg.IsActive,
                              State_Id = sg.State_Id,
                              DisplayName=sg.CompanyName+"("+s.StateName+")"
                          }
                         ).ToList();
            return states;
        }


    }
}