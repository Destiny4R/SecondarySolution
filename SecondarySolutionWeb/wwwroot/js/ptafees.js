$(document).ready(function () {
    datatablez = $("#studentdata").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "paging": true,
        responsive: true,
        "ajax": {
            "url": "/api/PTAFeesPayemtData",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": true,
            "searchable": true,
        }],
        "columns": [
            {
                "data": "id",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                "data": null, "render": function (data, type, full) {
                    var name = data.termregistration.studentsData.surName + ' ' + (data?.termregistration.studentsData.otherName || '') + ' ' + data.termregistration.studentsData.firstName;
                    return name;
                }
            },
            { "data": "ptaFee.fees", "autoWidth": true },
            { "data": "amount", "autoWidth": true },
            { "data": "balance", "autoWidth": true },
            { "data": "termregistration.term", "autoWidth": true },
            { "data": "termregistration.sessionYear.name", "autoWidth": true },
            {
                "data": null, "render": function (data, type, full) {
                    var name = data.termregistration.schoolclasses.name + ' - ' + (data.termregistration.subClasses.name || '');
                    return name;
                }
            },
            {"data": "createdDate",
                render: function (data, type, row) {
                    var hours = new Date(data).getHours()
                    let ap = hours >= 12 ? 'pm' : 'am';
                    return data = data.toLocaleString('YYYY-MM-dd').slice(0, 19).replace('T', ' ') + ' ' + ap;
                }
            }
        ]
    });
});
