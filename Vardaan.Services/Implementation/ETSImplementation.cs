using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
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
                        IsRouting = false,
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
                            where er.IsRouting==false
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
        public async Task<List<AvailableDriverDTO>> GetAvailableDrivers()
        {
            try
            {
                var driver = await (from d in ent.Drivers
                                    join dl in ent.DriverLoginHistories on d.Id equals dl.DriverId
                                    join di in ent.DriverDeviceIds on d.Id equals di.Driver_Id
                                    join c in ent.Cabs on dl.VehicleNumber equals c.VehicleNumber
                                    join cm in ent.VehicleModels on c.VehicleModel_Id equals cm.Id
                                    join v in ent.Vendors on d.Vendor_Id equals v.Id
                                    join com in ent.Customers on v.Company_Id equals com.Id
                                    join cap in ent.VehicleCapacities on c.VehicleCapacity_Id equals cap.Id
                                    where dl.IsActive == true
                                    select new AvailableDriverDTO
                                    {
                                        CheckinId = dl.Id,
                                        DriverId = d.Id,
                                        DeviceId = di.Id,
                                        DriverName = d.DriverName,
                                        MobileNumber = d.MobileNumber,
                                        VehicleNumber = c.VehicleNumber,
                                        VehicleModel = cm.ModelName,
                                        VendorName = v.CompanyName,
                                        VehicleMake = c.VehicleMake,
                                        Capacity = cap.Capacity,
                                        CompanyName = com.OrgName,
                                    }).ToListAsync();

                return driver;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAvailableDrivers: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Escorts>> GetChechinEscort()
        {
            try
            {
                var data = await (from e in ent.Escorts
                            join c in ent.Customers on e.CompanyId equals c.Id
                            where e.IsActive == true && e.IsCheckin==false || e.IsCheckin==null
                            select new Escorts
                            {
                                Id= e.Id,
                                EscortName= e.EscortName,
                                EscortMobileNumber= e.EscortMobileNumber,
                                EscortAddress= e.EscortAddress,
                                CompanyName= c.OrgName,
                            }
                            ).ToListAsync();

                if (data.Count > 0)
                {
                    return data;
                }
                return null;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetChechinEscort: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Escorts>> GetEscortAvailable()
        {
            try
            {
                var data = await (from e in ent.Escorts
                                  join c in ent.Customers on e.CompanyId equals c.Id
                                  where e.IsActive == true && e.IsCheckin==true
                                  select new Escorts
                                  {
                                      Id = e.Id,
                                      EscortName = e.EscortName,
                                      EscortMobileNumber = e.EscortMobileNumber,
                                      EscortAddress = e.EscortAddress,
                                      CompanyName = c.OrgName,
                                  }
                            ).ToListAsync();

                return data;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetChechinEscort: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> AddDriverCheckoutRemark(DriverCheckoutRemarkModel model)
        {
            try
            {
                var data = new DriverCheckoutRemark()
                {
                    Driver_Id= model.DriverId,
                    Remark= model.Remark,
                    RemarkDate= DateTime.Now
                };
                ent.DriverCheckoutRemarks.Add(data);
                ent.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<string> AddVehicleInspection(VehicleInspectionDTO model)
        {
            try
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["Vardaan_AdminEntities"].ConnectionString;
               
                using (var entityConnection = new EntityConnection(entityConnectionString))
                {
                    entityConnection.Open();
                    using (var command = entityConnection.CreateCommand())
                    {
                        command.CommandText = "Vardaan_AdminEntities.ManageVehicleInspection";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new EntityParameter("Vendor_Id", DbType.Int32) { Value = model.Vendor_Id });
                        command.Parameters.Add(new EntityParameter("Vehicle_Id", DbType.Int32) { Value = model.Vehicle_Id });
                        command.Parameters.Add(new EntityParameter("InspectionDate", DbType.DateTime) { Value = model.InspectionDate });
                        command.Parameters.Add(new EntityParameter("AC_Working", DbType.Boolean) { Value = model.AC_Working });
                        command.Parameters.Add(new EntityParameter("AC_Remarks", DbType.String) { Value = model.AC_Remarks ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("UnderInfluence", DbType.Boolean) { Value = model.UnderInfluence });
                        command.Parameters.Add(new EntityParameter("UnderInfluence_Remarks", DbType.String) { Value = model.UnderInfluence_Remarks });
                        command.Parameters.Add(new EntityParameter("Wiper_Seasonal", DbType.Boolean) { Value = model.Wiper_Seasonal });
                        command.Parameters.Add(new EntityParameter("Wiper_Remarks", DbType.String) { Value = model.Wiper_Remarks });
                        command.Parameters.Add(new EntityParameter("National_Permit", DbType.Boolean) { Value = model.National_Permit });
                        command.Parameters.Add(new EntityParameter("NationalPermit_Remarks", DbType.String) { Value = model.NationalPermit_Remarks });
                        command.Parameters.Add(new EntityParameter("Windshield_Broken", DbType.Boolean) { Value = model.Windshield_Broken });
                        command.Parameters.Add(new EntityParameter("Windshield_Remarks", DbType.String) { Value = model.Windshield_Remarks });
                        command.Parameters.Add(new EntityParameter("Trip_Type", DbType.Int32) { Value = model.Trip_Type });
                        command.Parameters.Add(new EntityParameter("Visible_Body_Dent", DbType.Boolean) { Value = model.Visible_Body_Dent });
                        command.Parameters.Add(new EntityParameter("BodyDent_Remarks", DbType.String) { Value = model.BodyDent_Remarks });
                        command.Parameters.Add(new EntityParameter("Seat_Belts_Working", DbType.Boolean) { Value = model.Seat_Belts_Working });
                        command.Parameters.Add(new EntityParameter("SeatBelts_Remarks", DbType.String) { Value = model.SeatBelts_Remarks });
                        command.Parameters.Add(new EntityParameter("GPS_Not_Available", DbType.Boolean) { Value = model.GPS_Not_Available });
                        command.Parameters.Add(new EntityParameter("GPS_Remarks", DbType.String) { Value = model.GPS_Remarks ?? (object)DBNull.Value });
                        command.Parameters.Add(new EntityParameter("State_Permit", DbType.Boolean) { Value = model.State_Permit });
                        command.Parameters.Add(new EntityParameter("StatePermit_Remarks", DbType.String) { Value = model.StatePermit_Remarks });
                        command.Parameters.Add(new EntityParameter("Unregistered_Drivers", DbType.Boolean) { Value = model.Unregistered_Drivers });
                        command.Parameters.Add(new EntityParameter("UnregisteredDrivers_Remarks", DbType.String) { Value = model.UnregisteredDrivers_Remarks });
                        command.Parameters.Add(new EntityParameter("Shift_Time", DbType.Int64) { Value = model.Shift_Time });
                        command.Parameters.Add(new EntityParameter("Dirty_Unclean_Vehicle", DbType.Boolean) { Value = model.Dirty_Unclean_Vehicle });
                        command.Parameters.Add(new EntityParameter("UncleanVehicle_Remarks", DbType.String) { Value = model.UncleanVehicle_Remarks });
                        command.Parameters.Add(new EntityParameter("Seat_Cover", DbType.Boolean) { Value = model.Seat_Cover });
                        command.Parameters.Add(new EntityParameter("SeatCover_Remarks", DbType.String) { Value = model.SeatCover_Remarks });
                        command.Parameters.Add(new EntityParameter("Headlights_Indicators", DbType.Boolean) { Value = model.Headlights_Indicators });
                        command.Parameters.Add(new EntityParameter("Headlights_Remarks", DbType.String) { Value = model.Headlights_Remarks });
                        command.Parameters.Add(new EntityParameter("Insurance", DbType.Boolean) { Value = model.Insurance });
                        command.Parameters.Add(new EntityParameter("Insurance_Remarks", DbType.String) { Value = model.Insurance_Remarks });
                        command.Parameters.Add(new EntityParameter("Unregistered_Cab", DbType.Boolean) { Value = model.Unregistered_Cab });
                        command.Parameters.Add(new EntityParameter("UnregisteredCab_Remarks", DbType.String) { Value = model.UnregisteredCab_Remarks });
                        command.Parameters.Add(new EntityParameter("City_Name", DbType.String) { Value = model.City_Name });
                        command.Parameters.Add(new EntityParameter("Driver_Uniform", DbType.Boolean) { Value = model.Driver_Uniform });
                        command.Parameters.Add(new EntityParameter("DriverUniform_Remarks", DbType.String) { Value = model.DriverUniform_Remarks });
                        command.Parameters.Add(new EntityParameter("Spare_Wheel", DbType.Boolean) { Value = model.Spare_Wheel });
                        command.Parameters.Add(new EntityParameter("SpareWheel_Remarks", DbType.String) { Value = model.SpareWheel_Remarks });
                        command.Parameters.Add(new EntityParameter("RC_Book", DbType.Boolean) { Value = model.RC_Book });
                        command.Parameters.Add(new EntityParameter("RCBook_Remarks", DbType.String) { Value = model.RCBook_Remarks });
                        command.Parameters.Add(new EntityParameter("Pollution", DbType.Boolean) { Value = model.Pollution });
                        command.Parameters.Add(new EntityParameter("Pollution_Remarks", DbType.String) { Value = model.Pollution_Remarks });
                        command.Parameters.Add(new EntityParameter("Penalty_Amount", DbType.String) { Value = model.Penalty_Amount });
                        command.Parameters.Add(new EntityParameter("Feedback", DbType.String) { Value = model.Feedback });
                        command.Parameters.Add(new EntityParameter("Fire_Extinguisher", DbType.Boolean) { Value = model.Fire_Extinguisher });
                        command.Parameters.Add(new EntityParameter("FireExtinguisher_Remarks", DbType.String) { Value = model.FireExtinguisher_Remarks });
                        command.Parameters.Add(new EntityParameter("Tool_Kit", DbType.Boolean) { Value = model.Tool_Kit });
                        command.Parameters.Add(new EntityParameter("ToolKit_Remarks", DbType.String) { Value = model.ToolKit_Remarks });
                        command.Parameters.Add(new EntityParameter("Fitness", DbType.Boolean) { Value = model.Fitness });
                        command.Parameters.Add(new EntityParameter("Fitness_Remarks", DbType.String) { Value = model.Fitness_Remarks });
                        command.Parameters.Add(new EntityParameter("Commercial_License", DbType.Boolean) { Value = model.Commercial_License });
                        command.Parameters.Add(new EntityParameter("CommercialLicense_Remarks", DbType.String) { Value = model.CommercialLicense_Remarks });
                        command.Parameters.Add(new EntityParameter("Penalty_Description", DbType.String) { Value = model.Penalty_Description });
                        command.Parameters.Add(new EntityParameter("First_Aid_Box", DbType.Boolean) { Value = model.First_Aid_Box });
                        command.Parameters.Add(new EntityParameter("FirstAidBox_Remarks", DbType.String) { Value = model.FirstAidBox_Remarks });
                        command.Parameters.Add(new EntityParameter("Fog_Lamp", DbType.String) { Value = model.Fog_Lamp });
                        command.Parameters.Add(new EntityParameter("FogLamp_Remarks", DbType.String) { Value = model.FogLamp_Remarks });
                        command.Parameters.Add(new EntityParameter("Passenger_Tax", DbType.Boolean) { Value = model.Passenger_Tax });
                        command.Parameters.Add(new EntityParameter("PassengerTax_Remarks", DbType.String) { Value = model.PassengerTax_Remarks });
                        command.Parameters.Add(new EntityParameter("Vehicle_Model_Over_5_Years", DbType.String) { Value = model.Vehicle_Model_Over_5_Years });
                        command.Parameters.Add(new EntityParameter("VehicleModel_Remarks", DbType.String) { Value = model.VehicleModel_Remarks });
                        command.Parameters.Add(new EntityParameter("Total_NC_Count", DbType.String) { Value = model.Total_NC_Count });
                        command.Parameters.Add(new EntityParameter("IsActive", DbType.Boolean) { Value = true });
                        command.Parameters.Add(new EntityParameter("InspectionByEmployeeId", DbType.String) { Value = model.InspectionByEmployeeId });

                        var responseMessageParam = new EntityParameter("ResponseMessage", DbType.String) { Direction = ParameterDirection.Output, Size = 255 };
                        command.Parameters.Add(responseMessageParam);

                        command.ExecuteNonQuery();

                        string responseMessage = responseMessageParam.Value.ToString();

                        return responseMessage;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
