using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;

namespace Vardaan.Services.Implementation
{
    public class ETSImplementation:IETS
    {
        Vardaan_AdminEntities ent =new Vardaan_AdminEntities();
        public async Task<bool> AddUpdateRequest(CreateRequestDTO model)
        {
			try
			{
                if (model.Id == 0)
                {
                    var EmpReq = new EmployeeRequest()
                    {
                        RequestType = model.RequestType,
                        CompanyId = model.CompanyId,
                        EmployeeId = model.EmployeeId,
                        Unit = model.Unit,
                        Department = model.Department,
                        CostCentre = model.CostCentre,
                        ExpenseCode = model.ExpenseCode,
                        RequestorEmpId = model.RequestorEmpId,
                        RequestorName = model.RequestorName,
                        RequestorContacts = model.RequestorContacts,
                        BookingReceivedDate = model.BookingReceivedDate,
                        RequestTripType = model.RequestTripType,
                        DestinationRequestMethod = model.DestinationRequestMethod,
                        LocationType = model.LocationType,
                        StartRequestDate = model.StartRequestDate,
                        EndRequestDate = model.EndRequestDate,
                        TripType = model.TripType,
                        ShiftType = model.ShiftType,
                        SMSTriggeredLocation = model.SMSTriggeredLocation,
                        PickupShiftTimeId = model.PickupShiftTimeId,
                        DropShiftTimeId = model.DropShiftTimeId,
                        CreatedDate = DateTime.Now

                    };
                    ent.EmployeeRequests.Add(EmpReq);
                }
                else
                {
                    var data = ent.EmployeeRequests.Find(model.Id);
                    data.RequestType = model.RequestType;
                    data.CompanyId = model.CompanyId;
                    data.EmployeeId = model.EmployeeId;
                    data.Unit = model.Unit;
                    data.Department = model.Department;
                    data.CostCentre = model.CostCentre;
                    data.ExpenseCode = model.ExpenseCode;
                    data.RequestorEmpId = model.RequestorEmpId;
                    data.RequestorName = model.RequestorName;
                    data.RequestorContacts = model.RequestorContacts;
                    data.BookingReceivedDate = model.BookingReceivedDate;
                    data.RequestTripType = model.RequestTripType;
                    data.DestinationRequestMethod = model.DestinationRequestMethod;
                    data.LocationType = model.LocationType;
                    data.StartRequestDate = model.StartRequestDate;
                    data.EndRequestDate = model.EndRequestDate;
                    data.TripType = model.TripType;
                    data.ShiftType = model.ShiftType;
                    data.SMSTriggeredLocation = model.SMSTriggeredLocation;
                    data.PickupShiftTimeId = model.PickupShiftTimeId;
                    data.DropShiftTimeId = model.DropShiftTimeId;
                }
                ent.SaveChanges();
                return true;
            }
			catch (Exception)
			{

				throw;
			}
        }
        public async Task<List<EmployeeRequests>> GetEmployeerequests()
        {
            try
            {
                var data = (from er in ent.EmployeeRequests
                            join e in ent.Employees on er.EmployeeId equals e.Employee_Id
                            join c in ent.Customers on er.CompanyId equals c.Id
                            join tt in ent.TripTypes on er.TripType equals tt.Id
                            join st in ent.TripMasters on er.ShiftType equals st.Id
                            orderby er.Id descending
                            select new EmployeeRequests
                            {
                                Id = er.Id,
                                CompanyName = c.CompanyName,
                                OrgName = c.OrgName,
                                RequestType = er.RequestType,
                                EmployeeId = er.EmployeeId,
                                Employee_Id = e.Id,
                                //FirstName = e.Employee_First_Name,
                                //LastName = e.Employee_Last_Name,
                                //Gender = e.Gender,
                                //Email = e.Email,
                                //GuestContact = e.MobileNumber,
                                Unit = er.Unit,
                                Department = er.Department,
                                CostCentre = er.CostCentre,
                                ExpenseCode = er.ExpenseCode,
                                RequestorEmpId = er.RequestorEmpId,
                                RequestorName = er.RequestorName,
                                RequestorContacts = er.RequestorContacts,
                                BookingReceivedDate = er.BookingReceivedDate,
                                RequestTripType = er.RequestTripType,
                                DestinationRequestMethod = er.DestinationRequestMethod,
                                LocationType = er.LocationType,
                                StartRequestDate = er.StartRequestDate,
                                EndRequestDate = er.EndRequestDate,
                                TripType = tt.TripTypeName,
                                ShiftType = st.TripName,
                                SMSTriggeredLocation = er.SMSTriggeredLocation,
                                CreatedDate = er.CreatedDate
                            }
                            ).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeleteRequest(int id)
        {
            try
            {
                var dt = ent.EmployeeRequests.Find(id);
                ent.EmployeeRequests.Remove(dt);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
