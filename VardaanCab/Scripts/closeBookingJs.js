/// <reference path="jquery-1.12.4.js" />
let oldFare = $("#oldFare").val();
let startingMinHour = parseInt($("#MinHrs").val());
let startingMinKm = parseInt($("#MinKm").val());
let initialStartHrs = $("#strartHrs").val();
let pickupDate = $("#pickupDate").val();

//calculateKms();
//calculateGt();
//calculateHours();

$("#StartKms,#EndKms").keyup(function () {
    calculateKms();
    calculateGt();
   
})
$('#StartHour').on('changeTime', function () {
    calculateHours();
});
$('#EndHour').on('changeTime', function () {
    calculateHours();
});

//bhupesh
$("#JourneyStartDate").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: 'yy-mm-dd',
    onSelect: function (dateText, inst) {
       
        calculateHours();

    }
});


$("#JourneyClosingDate").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: 'yy-mm-dd',
    onSelect: function (dateText, inst) {
        calculateHours();
          
    }
});

function calculateHours()
{
    let package8HourFare = $("#Package8HourFare").val() ? $("#Package8HourFare").val() : 0;
    let fare = $("#Fare");
    let txtBookingId = $("#Booking_Id");
    let bookingId = txtBookingId.val();
    let txtStartDate = $("#JourneyStartDate");
    let txtEndDate = $("#JourneyClosingDate")
    let txtStartHour = $("#StartHour");
    let txtEndHour = $("#EndHour");
    let txtTotalHours = $("#TotalHrs");
    let txtMinHours = $("#MinHrs");
    let txtMinKms = $("#MinKm");
    let txtExtraHours = $("#ExtraHrs");
    let txtExtraHourCharge = $("#ExtraHrsCharge");
    
    let txtStartKms = $("#StartKms");
    let txtEndKms = $("#EndKms");
    let txtExtraKms = $("#ExtraKms");
    let txtChargeExtraKm = $("#ExtraKmsCharge");
    let txtExtraHourRate = $("#ExtraHrsRate");
    let minKms = parseFloat(txtMinKms.val());
    let startKms = txtStartKms.val() ? parseInt(txtStartKms.val()) : 0;
    let endKms = txtEndKms.val() ? parseInt(txtEndKms.val()) : 0;
    let totalKms = endKms - startKms;

    let startDate = txtStartDate.val();
    let endDate = txtEndDate.val();
    let startHour = txtStartHour.val();
    let endHour = txtEndHour.val();
    if (endDate && startDate && endHour) {
        var data = {
            JourneyStartDate: startDate,
            JourneyEndDate: endDate,
            JourneyStartTime: startHour,
            JourneyEndTime: endHour
        };
        $.ajax({
            url: '/Booking/GetRemainingHour',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(data)
        }).done(function (r) {
           debugger
            let totalHrs =  parseFloat(r).toFixed(3);
            txtTotalHours.val(totalHrs);
            let minHours = parseInt(txtMinHours.val());
            var txtcompanyname = $('#companyname').val();
            if (txtcompanyname == "Adobe Systems India Pvt. Ltd." || txtcompanyname =="CUSHMAN & WAKEFIELD PROPERTY MANAGEMENT SERVICES INDIA PVT. LTD.")
            {
                if (minHours == 4 && totalHrs >= 6 && package8HourFare > 0) {
                    fare.val(package8HourFare);
                    minHours = 8;
                    minKms = 80;
                    txtMinKms.val(80);
                    txtMinHours.val(8);

                    txtExtraHours.val(0);
                    txtExtraHourCharge.val(0);
                    txtExtraKms.val(0);
                    txtChargeExtraKm.val(0);
                } else if (startingMinHour === 4 && minHours === 4 && totalHrs < 6) {
                    fare.val(oldFare);
                    minHours = 4;
                    minKms = 40;
                    txtMinKms.val(40);
                    txtMinHours.val(4);
                }
                else
                {
                    //fare.val(oldFare);
                    //minHours = 4;
                    //minKms = 40;
                    //txtMinKms.val(40);
                    //txtMinHours.val(4);
                }
            }
           else if (minHours == 4 && totalHrs >= 5 && package8HourFare > 0)
            {
                fare.val(package8HourFare);
                minHours = 8;
                minKms = 80;
                txtMinKms.val(80);
                txtMinHours.val(8);
            
                txtExtraHours.val(0);
                txtExtraHourCharge.val(0);
                txtExtraKms.val(0);
                txtChargeExtraKm.val(0);
            }
          
            else if (startingMinHour === 4 && minHours === 4 && totalHrs < 5)
            {
                fare.val(oldFare);
                minHours = 4;
                minKms = 40;
                txtMinKms.val(40);
                txtMinHours.val(4);
            }
            else if (startingMinHour === 4 && minHours === 8 && totalHrs < 5)
            {
                if (totalKms <= 50)
                {
                    minHours = 4;
                    minKms = 40;
                    txtMinKms.val(40);
                    txtMinHours.val(4);
                }
                 let extraKms = 0;
                if (totalKms > minKms)
                {
                extraKms = totalKms - minKms;
                txtExtraKms.val(extraKms);
                    let ratePerExtraKm = parseFloat($("#ExtraKmsRate").val());
                let extraCharge = extraKms * ratePerExtraKm;
                txtChargeExtraKm.val(extraCharge);
                }
            }
            else if (startingMinHour === 4 && minHours === 8 && totalHrs <= 8)
            {
                txtExtraHours.val(0);
                txtExtraHourCharge.val(0);
                txtExtraHourRate.val(0);
               
            }
            else
            {
                //alert("test");
            }

            if (startingMinHour == minHours)
            {
                fare.val(oldFare);
                minHours = startingMinHour;
                txtMinKms.val(startingMinKm);
                txtMinHours.val(startingMinHour);

            }
            txtExtraHours.val(0);
            txtExtraHourCharge.val(0);
            if (totalHrs > minHours)
            {
                let extraTime = totalHrs - minHours;
             //   alert("ExtraTime - "+extraTime);
                txtExtraHours.val(extraTime.toFixed(3));
                let extraTimeInMinutes = extraTime * 60;
             //   alert("ExtraTimeInMinutes - " +extraTimeInMinutes);
                //rates
                let ratePerHour = parseFloat(txtExtraHourRate.val());
               
                let ratePerMin = ratePerHour / 60;
                //let ratePerQuarterHour = ratePerMin*15;//old logic
                //let ratePerQuarterHour = ratePerMin;

                let extraCompleteHour = Math.round(extraTimeInMinutes);
             //   alert("ExtraTime In Minutes"+ extraCompleteHour);
                let remainMin = extraTimeInMinutes;
                //let remainMin = extraTimeInMinutes % 60;//old logic
               // let quarterlyCharge = Math.ceil(remainMin / 15) * ratePerQuarterHour;//old logic
               // let quarterlyCharge = (Math.ceil(remainMin)/60) * ratePerQuarterHour;

                //let extraCharge = Math.round((extraCompleteHour * ratePerHour) + quarterlyCharge);////old logic
                let extraCharge = Math.round((Math.round(remainMin) / 60) * ratePerHour);
             //   alert("ExtraCharage - "+extraCharge);
                txtExtraHourCharge.val(extraCharge);
            }
            calculateGt();
        })
            .fail(function (err) {
                console.log(err.responseText);
            });
    }
}

function calculateKms()
{
    
    let package8HourFare = $("#Package8HourFare").val() ? $("#Package8HourFare").val() : 0;
    //let oldFare = $("#oldFare").val();
    let fare = $("#Fare");
    let txtStartKms = $("#StartKms");
    let txtEndKms = $("#EndKms");
    let txtExtraKms = $("#ExtraKms");
    let txtChargeExtraKm = $("#ExtraKmsCharge");
    txtExtraKms.val(0);
    txtChargeExtraKm.val(0);
    let txtTotalHours = $("#TotalHrs");
    let txtExtraHours = $("#ExtraHrs");
    let txtExtraHourCharge = $("#ExtraHrsCharge");
    let startKms = txtStartKms.val() ? parseInt(txtStartKms.val()) : 0;
    let endKms = txtEndKms.val() ? parseInt(txtEndKms.val()) : 0;

    var txtTotalKms = $("#TotalKms");
    let totalKms = endKms - startKms;
    txtTotalKms.val(totalKms);
    let txtMinKms = $("#MinKm");
    let txtMinHours = $("#MinHrs");
    let minKms = parseFloat(txtMinKms.val());
    /////////

    // hour textboxes
    let txtStartDate = $("#JourneyStartDate");
    let txtEndDate = $("#JourneyClosingDate")
    let txtStartHour = $("#StartHour");
    let txtEndHour = $("#EndHour");

    var txtcompanyname = $('#companyname').val();
   
    //alert(package8HourFare);
    /////
    if (txtcompanyname == "Adobe Systems India Pvt. Ltd." || txtcompanyname =="CUSHMAN & WAKEFIELD PROPERTY MANAGEMENT SERVICES INDIA PVT. LTD.")
    {
        if (minKms == 40 && totalKms >=60 && package8HourFare > 0) {

            fare.val(package8HourFare);
            minKms = 80;
            txtMinKms.val(80);
            txtExtraKms.val(0);
            txtTotalHours.val(0);
            txtChargeExtraKm.val(0); //
            txtMinHours.val(8);
        } else {
            fare.val(oldFare);
            minKms = startingMinKm;
            txtMinKms.val(startingMinKm);
            txtMinHours.val(startingMinHour);
        }
    }
    else if (minKms == 40 && totalKms >= 50 && package8HourFare > 0)
    {
            fare.val(package8HourFare);
            minKms = 80;
            txtMinKms.val(80);
            txtExtraKms.val(0);
            txtTotalHours.val(0);
            txtChargeExtraKm.val(0); //
            txtMinHours.val(8);
       
    }
    else
    {
        fare.val(oldFare);
        minKms = startingMinKm;
        txtMinKms.val(startingMinKm);
        txtMinHours.val(startingMinHour);
    }
    /////////
    // reset start and closing date data

    //txtStartDate.val(pickupDate);
    //txtEndDate.val("");
    //txtStartHour.val(initialStartHrs);
    //txtEndHour.val("00:00:00");
    //txtTotalHours.val(0);
    //txtExtraHours.val(0);
    //txtExtraHourCharge.val(0);

    //end reset

    let extraKms = 0;
    if (totalKms > minKms) {
        extraKms = totalKms - minKms;
        txtExtraKms.val(extraKms);
        let ratePerExtraKm = parseFloat($("#ExtraKmsRate").val());
        let extraCharge = extraKms * ratePerExtraKm;
        txtChargeExtraKm.val(extraCharge);
    }   
}

$("#IncreaseDecreaseInFuel").keyup(function () {debugger
    
    let txtInc = $(this);
    let txtFuelCharge = $("#FuelCharge");
    let inc = txtInc.val() ? parseFloat(txtInc.val()) : 0;
    let txtFuelEff = $("#FuelEfficiency");
    let fuelEff = txtFuelEff.val() ? parseFloat(txtFuelEff.val()) : 0;
    var txtTotalKms = $("#TotalKms");
    let totalKm = txtTotalKms.val() ? parseFloat(txtTotalKms.val()) : 0;
    let fuelCharge = fuelEff==0?0:(totalKm / fuelEff) * inc;
    txtFuelCharge.val(fuelCharge);
    calculateGt();
})

$("#TollCharge,#ParkCharge,#StateCharge,#OutStationDays,#InterStateCharge,#DiscountValue,#MCD,#MiscCharge").keyup(function () {
    calculateGt();
})


$("#TotalNight").keyup(function () {
    let txtTotalNight = $(this);
    let txtRatePerNight = $("#NightRate");
    let txtNightCharges = $("#NightCharge");
    let totalNights = txtTotalNight.val() ? parseInt(txtTotalNight.val()) : 0;
    let ratePerNight = txtRatePerNight.val() ? parseFloat(txtRatePerNight.val()) : 0;
    let totalCharge = Math.round(ratePerNight * totalNights);
    txtNightCharges.val(totalCharge);
    calculateGt();
})

$("#DiscountType").change(function () {
    calculateGt();
})

function calculateGt() {
    let txtExtraKmCharge = $("#ExtraKmsCharge");
    let txtExtraHourCharge = $("#ExtraHrsCharge");
    let txtFare = $("#Fare");
    let txtNightCharge = $("#NightCharge");
    let txtFuelCharge = $("#FuelCharge");
    let txtTollCharge = $("#TollCharge");
    let txtStateCharge = $("#StateCharge");
    let txtParkingCharge = $("#ParkCharge");
    let txtOutstationRatePerDay = $("#OutStationChargeRate");
    let txtOutstationDays = $("#OutStationDays");
    let txtOutstationCharge = $("#OutStationCharge");
    let txtInterstateCharges = $("#InterStateCharge");
    let txtMisc = $("#MiscCharge"); //
    let txtMcd = $("#MCD");
    let fare = parseFloat(txtFare.val());
    let extraKmCharge = parseFloat(txtExtraKmCharge.val());
    let extraHourCharge = parseFloat(txtExtraHourCharge.val());
    let nightCharges = parseFloat(txtNightCharge.val());
    let tollCharge = txtTollCharge.val()?parseFloat(txtTollCharge.val()):0;
    let parkingCharge = txtParkingCharge.val() ? parseFloat(txtParkingCharge.val()) : 0;
    let stateCharge = txtStateCharge.val() ? parseFloat(txtStateCharge.val()) : 0;
    let fuelCharge = parseFloat(txtFuelCharge.val());
    let outStationPerDayCharge = parseFloat(txtOutstationRatePerDay.val());
    let outstationDays = parseFloat(txtOutstationDays.val());
    let outstationCharges = outStationPerDayCharge * outstationDays;
    txtOutstationCharge.val(outstationCharges);
    let interstateCharges = parseFloat(txtInterstateCharges.val());
    let mcd = parseFloat(txtMcd.val() ? txtMcd.val() : 0);
    let misc = parseFloat(txtMisc.val() ? txtMisc.val() : 0);//
    let finalCharge = fare+misc +mcd+ extraKmCharge + extraHourCharge + nightCharges + tollCharge+stateCharge+parkingCharge + fuelCharge+interstateCharges+outstationCharges;
    let txtNetAmount = $("#netAmount");
    txtNetAmount.val(finalCharge);
    let netAmt = parseFloat(finalCharge);
    let ddDiscountType = $("#DiscountType");
    let txtDiscountValue = $("#DiscountValue");
    let txtDiscountedAmount = $("#DiscountAmount");
    let gt = netAmt.toFixed(3);
    $("#Total").val(gt);

    if (ddDiscountType.val())
    {
        var discountVal = parseFloat(txtDiscountValue.val());
        var discountAmt = 0;
        var dt = ddDiscountType.val();
        var discountableAmount = fare + extraKmCharge + extraHourCharge;

        if(dt=="Flat")
        {
            discountAmt = discountVal;
        }
        else {
            discountAmt = parseFloat((discountableAmount * discountVal) / 100).toFixed(3);
        }
        txtDiscountedAmount.val(discountAmt);

        if(discountAmt>0)
        {
            let discountedAmt = netAmt - discountAmt;
            gt = discountedAmt.toFixed(3);
        }
    }
    
    $("#Total").val(gt);
}