

/// <reference path="jquery-1.12.4.js" />
let oldFare = $("#oldFare").val();
let startingMinHour = parseInt($("#MinHrs").val());
let startingMinKm = parseInt($("#MinKm").val());
let initialStartHrs = $("#strartHrs").val();
let pickupDate = $("#pickupDate").val();
let txtTotalMinKm = $("#totalMinKms");
let txtOutstationDays = $("#OutStationDays");
let farePerday = parseFloat($("#oldFare").val());
$("#StartKms,#EndKms").keyup(function () {
    calculateKms();
    calculateGt()
})


$("#JourneyStartDate").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: 'yy-mm-dd'
});

$("#JourneyClosingDate").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: 'yy-mm-dd',
    onSelect: function (dateText, inst) {
        var txtJourneyStart = $("#JourneyStartDate");
        var sDate = txtJourneyStart.val();
        var txtDays = $("#OutStationDays");
        $.get(`/Booking/GetDays?startDays=${sDate}&endDays=${dateText}`)
            .then(function (r) {
                txtDays.val(r);
                calculateKms();
                calculateGt();
            }
            ).fail(function (e) {
                console.log(e.responseText);
                calculateGt();

            });
    }
});



function calculateKms() {
    
    let package8HourFare = $("#Package8HourFare").val() ? $("#Package8HourFare").val() : 0;
    let outstationDays = !txtOutstationDays.val() ? 1 : txtOutstationDays.val() <= 0 ? 1 : txtOutstationDays.val();
   
    //let oldFare = $("#oldFare").val();
    let fare = $("#Fare");
    let totalFare = farePerday * outstationDays;
    fare.val(totalFare);
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
    let totalMinKms = minKms * outstationDays;
    txtTotalMinKm.val(totalMinKms);
   
    let extraKms = 0;
    if (totalKms > totalMinKms) {
        extraKms = totalKms - totalMinKms;
        txtExtraKms.val(extraKms);
        let ratePerExtraKm = parseFloat($("#ExtraKmsRate").val());
        let extraCharge = extraKms * ratePerExtraKm;
        txtChargeExtraKm.val(extraCharge);
    }
}

$("#IncreaseDecreaseInFuel").keyup(function () {
    debugger

    let txtInc = $(this);
    let txtFuelCharge = $("#FuelCharge");
    let inc = txtInc.val() ? parseFloat(txtInc.val()) : 0;
    let txtFuelEff = $("#FuelEfficiency");
    let fuelEff = txtFuelEff.val() ? parseFloat(txtFuelEff.val()) : 0;
    var txtTotalKms = $("#TotalKms");
    let totalKm = txtTotalKms.val() ? parseFloat(txtTotalKms.val()) : 0;
    let fuelCharge = fuelEff == 0 ? 0 : (totalKm / fuelEff) * inc;
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
    let txtMcd = $("#MCD");
    let txtMisc = $("#MiscCharge");

    let fare = parseFloat(txtFare.val());
    let extraKmCharge = parseFloat(txtExtraKmCharge.val());
    let extraHourCharge = parseFloat(txtExtraHourCharge.val());
    let nightCharges = parseFloat(txtNightCharge.val());
    let tollCharge = txtTollCharge.val() ? parseFloat(txtTollCharge.val()) : 0;
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
    let finalCharge = Math.round(fare+misc + mcd + extraKmCharge + extraHourCharge + nightCharges + tollCharge + stateCharge + parkingCharge + fuelCharge + interstateCharges + outstationCharges);
    let txtNetAmount = $("#netAmount");
    txtNetAmount.val(finalCharge);
    let netAmt = parseFloat(finalCharge);
    let ddDiscountType = $("#DiscountType");
    let txtDiscountValue = $("#DiscountValue");
    let txtDiscountedAmount = $("#DiscountAmount");
    let gt = netAmt;
    $("#Total").val(gt);

    if (ddDiscountType.val()) {
        var discountVal = parseFloat(txtDiscountValue.val());
        var discountAmt = 0;
        var dt = ddDiscountType.val();
        var discountableAmount = fare + extraKmCharge + extraHourCharge;
        if (dt == "Flat") {
            discountAmt = discountVal;
        }
        else {
            discountAmt = parseFloat((discountableAmount * discountVal) / 100).toFixed(2);
        }
        txtDiscountedAmount.val(discountAmt);
        if (discountAmt > 0) {
            let discountedAmt = netAmt - discountAmt;
            gt = discountedAmt;
        }
    }

    $("#Total").val(gt);
}