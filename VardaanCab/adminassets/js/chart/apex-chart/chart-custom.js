//////pie chart for visitors
var options = {
    series: [44, 55, 41, 17],
    labels: ['The Passersby', 'The Occasionals', 'The Regulars', 'The Superfans'],
    chart: {
        width: "100%",
        height: 275,
        type: 'donut',
    },
    colors: ['#4aa4d9', '#ef3f3e', '#9e65c2', '#6670bd', '#FF9800'],
    plotOptions: {
        pie: {
            startAngle: -90,
            endAngle: 270
        }
    },
    dataLabels: {
        enabled: false
    },

    legend: {
        formatter: function (val, opts) {
            return val + " - " + opts.w.globals.series[opts.seriesIndex]
        },
        offsetY: 10,
    },
    responsive: [
        {

            breakpoint: 1835,
            options: {
                chart: {
                    height: 245,
                },
                legend: {

                    position: 'bottom',

                    itemMargin: {
                        horizontal: 5,
                        vertical: 1
                    },
                },

            },



        }, {

            breakpoint: 1388,
            options: {
                chart: {
                    height: 330,
                },
                legend: {

                    position: 'bottom',

                },

            },



        },
        {

            breakpoint: 1275,
            options: {
                chart: {
                    height: 300,
                },
                legend: {

                    position: 'bottom',

                },

            },



        },
        {

            breakpoint: 1158,
            options: {
                chart: {
                    height: 280,
                },

                legend: {
                    fontSize: '10px',
                    position: 'bottom',
                    offsetY: 10,
                },



            },



        },
        {

            breakpoint: 598,
            options: {
                chart: {
                    height: 280,
                },





                legend: {
                    fontSize: '12px',
                    position: 'bottom',
                    offsetX: 5,
                    offsetY: -5,


                    markers: {
                        width: 10,
                        height: 10,


                    },

                    itemMargin: {

                        vertical: 1
                    },
                },
            },



        },

    ],


};

var chart = new ApexCharts(document.querySelector("#pie-chart-visitors"), options);
chart.render();



// //basic area char visiter
// var options = {
//     series: [{
//         name: 'AED 500.00',
//         data: [20, 40, 10, 30, 25, 35],
//         // , 50, 20, 40, 25, 60, 0
//     }],
//     chart: {
//         height: 300,
//         type: 'area',
//         dropShadow: {
//             enabled: true,
//             top: 10,
//             left: 0,
//             blur: 3,
//             color: '#720f1e',
//             opacity: 0.15
//         },
//         toolbar: {
//             show: false
//         },
//         zoom: {
//             enabled: false
//         },
//     },
//     markers: {
//         strokeWidth: 4,
//         strokeColors: "#ffffff",
//         hover: {
//             size: 9,
//         }
//     },
//     dataLabels: {
//         enabled: false
//     },
//     stroke: {
//         curve: 'smooth',
//         lineCap: 'butt',
//         width: 4,
//     },
//     legend: {
//         show: false
//     },
//     colors: ["#4aa4d9"],
//     fill: {
//         type: 'gradient',
//         gradient: {
//             shadeIntensity: 1,
//             opacityFrom: 0.5,
//             opacityTo: 0.4,
//             stops: [0, 90, 100]
//         }
//     },
//     grid: {
//         xaxis: {
//             lines: {
//                 borderColor: 'transparent',
//                 show: false
//             }
//         },
//         yaxis: {
//             lines: {
//                 borderColor: 'transparent',
//                 show: false,
//             }

//         },
//         padding: {
//             right: -25,
//             bottom: 10,
//             left: -10,
//             top: -10
//         }
//     },
//     responsive: [{
//         breakpoint: 1200,
//         options: {
//             grid: {
//                 padding: {
//                     right: -95,
//                 }
//             },
//         },
//     },
//     {
//         breakpoint: 992,
//         options: {
//             grid: {
//                 padding: {
//                     right: -69,
//                 }
//             },
//         },
//     }
//     ],
//     yaxis: {
//         labels: {
//             show: false,
//             formatter: function (value) {
//                 return "AED " + value;
//             }
//         },
//         crosshairs: {
//             show: true,
//             position: 'back',
//             stroke: {
//                 color: '#b6b6b6',
//                 width: 1,
//                 dashArray: 5,
//             },
//         },
//         tooltip: {
//             enabled: false,
//         },
//     },
//     // "Thu", "Fri", "Sat", "Sun", "Mon", "Tue", "Wed", "Thu",
//     xaxis: {
//         categories: ["Sun", "Mon", "Tue", "Wed", "Thu", "wed"],
//         labels: {
//             offsetX: 10,
//         },


//         range: undefined,
//         axisBorder: {


//             show: false,
//         },
//         axisTicks: {
//             show: false,
//         },

//     },


//     tooltip: {
//         formatter: undefined,
//         custom: function ({ series, seriesIndex, dataPointIndex, w }) {
//             return '<div class="arrow_box">' +
//                 '<h4>' + "AED 500.00" + '</h4> ' +
//                 // '<span>' + series[seriesIndex][dataPointIndex] + '</span>' +
//                 '<span>' + series[seriesIndex][dataPointIndex] + '</span>' +
//                 '</div>'
//         }
//     },
// };

// var chart = new ApexCharts(document.querySelector("#basic-line-chart-visiters"), options);
// chart.render();


////bar chart
var options = {
    series: [{
        color: ["#4aa4d9"],
        name: 'Net Profit',
        data: [44, 55, 57, 56, 61, 58, 63, 60, 66]
    }, {
        name: 'Revenue',
        data: [76, 85, 101, 98, 87, 105, 91, 114, 94]
    }, {
        name: 'Free Cash Flow',
        data: [35, 41, 36, 26, 45, 48, 52, 53, 41]
    }],
    colors: ["#4aa4d9", "#747dc6", "#ef3f3e"],
    chart: {
        type: 'bar',
        height: 300
    },
    plotOptions: {
        bar: {
            horizontal: false,
            columnWidth: '55%',
            endingShape: 'rounded'
        },
    },
    dataLabels: {
        enabled: false
    },

    stroke: {
        show: true,
        width: 2,
        colors: ['transparent']
    },
    xaxis: {
        categories: ['Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'],
    },
    yaxis: {
        title: {
            text: '$ (thousands)'
        }
    },
    legend: { offsetY: -10, },
    fill: {
        opacity: 1
    },
    tooltip: {
        y: {
            formatter: function (val) {
                return "$ " + val + " thousands"
            }
        }
    },

    responsive: [
        {
            breakpoint: 1837,
            options: {
                chart: {
                    height: 323,

                },
                legend: {

                    position: 'bottom',

                },

            },
        },
        {
            breakpoint: 1572,
            options: {
                chart: {
                    height: 318,

                },
                legend: {

                    position: 'bottom',

                },

            },
        },
        {

            breakpoint: 1525,
            options: {
                chart: {
                    height: 345,

                },
                legend: {

                    position: 'bottom',

                },

            },
        },
        {

            breakpoint: 1463,
            options: {
                chart: {
                    height: 368,

                },
                legend: {

                    position: 'bottom',

                },

            },
        },
        {

            breakpoint: 1387,
            options: {
                chart: {
                    height: 343,

                },
                legend: {

                    position: 'bottom',

                },

            },
        },
        {

            breakpoint: 1275,
            options: {
                chart: {
                    height: 312,
                },
                legend: {

                    position: 'bottom',

                },

            },



        },
        {

            breakpoint: 650,
            options: {
                chart: {
                    height: 250,

                },




            },



        },
        {

            breakpoint: 584,
            options: {
                chart: {
                    height: 200,

                    offsetX: -10,
                },

                legend: {
                    fontSize: '12px',
                    position: 'bottom',
                    offsetX: 5,
                    MarginBottom: "5px",
                    horizontalAlign: 'center',
                    markers: {
                        width: 10,
                        height: 10,


                    },


                },

                yaxis: {
                    tooltip: {
                        enabled: false,
                        offsetX: 0,
                    },
                }


            },



        },
        {

            breakpoint: 438,
            options: {
                chart: {
                    height: 200,


                },




            },



        },
        {

            breakpoint: 371,
            options: {
                chart: {
                    height: 200,
                    width: "115%",
                    offsetX: -25,
                },

                legend: {

                    offsetY: -5,



                },


            },



        },





    ],




};

var chart = new ApexCharts(document.querySelector("#bar-chart-earning"), options);
chart.render();

///total-earnings-chart

var options = {
    series: [{
        name: "STOCK ABC",
        data: series.monthDataSeries1.prices
    }],
    chart: {
        type: 'area',
        height: 90,
        width: 90,
        zoom: {
            enabled: false
        }
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        curve: 'straight'
    },

    labels: series.monthDataSeries1.dates,
    xaxis: {
        labels: {
            show: false,

        },
        type: 'datetime',
        axisBorder: {


            show: false,
        },
        axisTicks: {
            show: false,
        },
    },
    yaxis: {
        labels: {
            show: false,

        },
        opposite: true
    },
    grid: {
        xaxis: {
            lines: {
                borderColor: 'transparent',
                show: false
            }

        },
        yaxis: {
            lines: {
                borderColor: 'transparent',
                show: false,
            }

        },
        padding: {
            right: 0,
            bottom: 0,
            left: 0,
            top: 0
        }
    },
    legend: {
        horizontalAlign: 'left'
    },

};

var chart = new ApexCharts(document.querySelector("#total-earnings-chart"), options);
chart.render();

////traffic chart

var generateDayWiseTimeSeries = function (baseval, count, yrange) {
    var i = 0;
    var series = [];
    while (i < count) {
        var x = baseval;
        var y = Math.floor(Math.random() * (yrange.max - yrange.min + 1)) + yrange.min;

        series.push([x, y]);
        baseval += 88400000;
        i++;
    }
    return series;
}




var options = {
    series: [
        {
            name: 'Referral Traffic',
            data: [45, 40, 140, 70, 150, 260, 240, 380, 110, 180, 270, 115, 70, 65, 50]
        },

    ],
    chart: {
        type: 'area',
        height: 370,
        stacked: true,
        events: {
            selection: function (chart, e) {
                console.log(new Date(e.xaxis.min))
            }
        },
    },
    stroke: {
        show: true,
        curve: 'smooth',
        lineCap: 'butt',
        colors: undefined,

        dashArray: 0,

    },
    markers: {
        size: 6,
        colors: '#fff',
        strokeColors: '#747dc6',
        strokeWidth: 4,
        strokeOpacity: 0.9,
        strokeDashArray: 0,
        fillOpacity: 1,
        discrete: [],
        shape: "circle",
        radius: 2,
        offsetX: 0,
        offsetY: 0,
        onClick: undefined,
        onDblClick: undefined,
        showNullDataPoints: true,
        hover: {
            size: undefined,
            sizeOffset: 3
        }
    },
    colors: ['#747dc6'],
    dataLabels: {
        enabled: false
    },
    grid: {

        padding: {
            right: 0,
            bottom: 0,
            left: 0,
            top: 0
        }
    },
    fill: {
        type: 'gradient',
        gradient: {
            opacityFrom: 0.6,
            opacityTo: 0.2,
            shade: 'light',
            type: "vertical",
            // optional, if not defined - uses the shades of same color in series
        }
    },
    legend: {
        position: 'top',
        horizontalAlign: 'left'
    },
    xaxis: {
        categories: ['jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec', 'jan', 'Feb', 'mar'],
    },
    responsive: [

        {
            breakpoint: 1400,
            options: {
                chart: {
                    height: 210,
                    width: "120%",
                    offsetX: -45,

                },
                legend: {

                    position: 'bottom',

                },
                dataLabels: {
                    textAnchor: 'left',

                    style: {
                        fontSize: '10px',

                    },
                }

            },
        },
        {
            breakpoint: 992,
            options: {
                chart: {
                    height: 210,
                    width: "100%",
                    offsetX: 0,

                },


            },
        },
        {
            breakpoint: 578,
            options: {
                chart: {
                    height: 200,
                    width: "105%",
                    offsetX: -20,
                    offsetY: 10,

                },


            },
        },
        {
            breakpoint: 430,
            options: {
                chart: {

                    width: "108%",


                },


            },
        },
        {
            breakpoint: 330,
            options: {
                chart: {

                    width: "112%",


                },


            },
        },
    ],
};

var chart = new ApexCharts(document.querySelector("#traffic-chart"), options);
chart.render();