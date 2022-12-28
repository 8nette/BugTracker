google.charts.setOnLoadCallback(drawAverageDefectAges);

function drawAverageDefectAges() {
    $.ajax({
        url: '/Survey/GetJsonStatAverageDefectAges',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            result = [];
            $.each(response, function (index, averageDefectAge) {
                result.push(['' + averageDefectAge.week + ', ' + averageDefectAge.year,
                averageDefectAge.averageDefectAge]);
                console.log(averageDefectAge.week + ", " + averageDefectAge.year);
            });

            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Week');
            data.addColumn('number', 'Average Bug Age in Days');
            data.addRows(result);

            var options = {
                title: 'Average Defect Age',
                backgroundColor: { fill: 'transparent' },
                colors: ["#608f82"],
                legend: 'right',
                width: '1200',
                height: '300',
                chartArea: {
                    width: '900',
                    left: '100'
                }
            };

            var chart = new google.visualization.LineChart($("#averageDefectAge")[0]);
            chart.draw(data, options);
        }
    });
}