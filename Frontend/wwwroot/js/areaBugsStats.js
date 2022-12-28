google.charts.setOnLoadCallback(drawAreaBugs);

function drawAreaBugs() {
    $.ajax({
        url: '/Survey/GetJsonAreaBugs',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            result = [];
            $.each(response, function (index, areaBugs) {
                result.push([areaBugs.area, areaBugs.generatedBugs, areaBugs.openBugs]);
            });

            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Area');
            data.addColumn('number', 'Number of Generated Bugs');
            data.addColumn('number', 'Number of Open Bugs');
            data.addRows(result);

            var options = {
                title: 'Generated and Open Bugs pr Area',
                backgroundColor: { fill: 'transparent' },
                colors: ['#608f82', '#89d6c0'],
                chartArea: {
                    width: '75%',
                    height: '10em'
                },
                height: '300'
            };

            var chart = new google.visualization.ColumnChart($("#areaBugs")[0]);
            chart.draw(data, options);
        }
    });
}