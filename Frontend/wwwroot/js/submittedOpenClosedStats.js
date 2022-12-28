google.charts.setOnLoadCallback(drawSubmittedOpenClosed);

function drawSubmittedOpenClosed() {
    $.ajax({
        url: '/Survey/GetJsonSubmittedOpenClosedStats',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            result = [];
            $.each(response, function (index, submittedOpenClosedBugs) {
                result.push(['' + submittedOpenClosedBugs.week + ', ' + submittedOpenClosedBugs.year,
                    submittedOpenClosedBugs.numberOfSubmittedBugs,
                    submittedOpenClosedBugs.numberOfClosedBugs,
                    submittedOpenClosedBugs.numberOfOpenBugs,]);
            });

            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Week');
            data.addColumn('number', 'Number of Submitted Bugs');
            data.addColumn('number', 'Number of Closed Bugs');
            data.addColumn('number', 'Number of Open Bugs');
            data.addRows(result);

            var options = {
                title: 'Submitted, Open and Closed Bugs',
                backgroundColor: { fill: 'transparent' },
                colors: ["#608f82", "#a7d1c5", "#89d6c0"],
                legend: 'right',
                width: '1200',
                height: '300',
                chartArea: {
                    width: '900',
                    left: '100'
                },
                vAxis: { title: 'Bugs' },
                hAxis: { title: 'Week' },
                seriesType: 'bars',
                series: { 2: { type: 'line' }}
            };

            var chart = new google.visualization.ComboChart($("#submittedOpenClosed")[0]);
            chart.draw(data, options);
        }
    });
}