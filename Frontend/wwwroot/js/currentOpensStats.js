google.charts.setOnLoadCallback(drawCurrentOpens);

function drawCurrentOpens() {
    $.ajax({
        url: '/Survey/GetJsonStatCurrentOpen',
        dataType: "json",
        type: "GET",
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            toastr.error(err.message);
        },
        success: function (response) {
            result = [];
            $.each(response, function (index, currentOpen) {
                result.push([new Date(currentOpen.day),
                currentOpen.numberOfOpenBugs]);
            });

            var data = new google.visualization.DataTable();
            data.addColumn('date', 'Time');
            data.addColumn('number', 'Open Bugs');
            data.addRows(result);

            var options = {
                title: 'Amount of not Closed Bugs Over Time',
                backgroundColor: { fill: 'transparent' },
                colors: ["#608f82"],
                legend: 'right',
                width: '1200',
                height: '300',
                lineWidth: 2,
                chartArea: {
                    width: '1000',
                    left: '100'
                },
                legend: {
                    position: 'none'
                }
            };

            var chart = new google.visualization.LineChart($("#currentOpen")[0]);
            chart.draw(data, options);
        }
    });
}