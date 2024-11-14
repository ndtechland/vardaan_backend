(function ($) {
    "use strict";


    // Time only
    $(function () {
        $('#CheckIn').datetimepicker({
            format: 'LT'
        });
    });
    $(function () {
        $('#CheckOut').datetimepicker({
            format: 'LT'
        });
    });
    $(function () {
        $('#Opening').datetimepicker({
            format: 'LT'
        });
    });
    $(function () {
        $('#Closing').datetimepicker({
            format: 'LT'
        });
    });

    //Date only
    $(function () {
        $('#dt-date').datetimepicker({
            format: 'L'
        });
    });


})(jQuery);