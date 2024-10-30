$(document).ready(function () {
    loadtable();
});
function loadtable() {
    datatable = $("#fee_payment").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        responsive: true,
        destroy: true,
        "ajax": {
            "url": "/api/FeesPayemtData",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{ "visible": true, "searchable": true, }],
        "columns": [
            {
                "data": "id", render: function (data, type, row, meta) { return meta.row + meta.settings._iDisplayStart + 1; }
            },
            {
                "data": null, "render": function (data, type, full) {
                    var name = data.termregistrations.studentsData.surName + ' ' + (data?.termregistrations.studentsData.otherName || '') + ' ' + data.termregistrations.studentsData.firstName;
                    return name;
                }
            },
            { "data": "termlyFees.amount", "autoWidth": true },
            { "data": "paid", "autoWidth": true },
            { "data": "balance", "autoWidth": true },
            {
                "data": null, "render": function (data, type, full) {
                    if (data.status == "Completed") {
                        return `<span class="badge h4 bg-success" title="Fees completed">
                        <i class="fa fa-check-square-o" aria-hidden="true"></i> Completed
                        </span>`;
                    } else {
                        return `<label title="Part Payment" class="badge h3 bg-warning"><i class="fa fa-star-half-o" aria-hidden="true"></i> Part Payment</label>`;
                    }
                }
            },
            {
                "data": null, "render": function (data, type, full) {
                    return data.termregistrations.sessionYear.name;
                }
            },
            {
                "data": null, "render": function (data, type, full) {
                    return data.termregistrations.term;
                }
            },
            {
                "data": null, "render": function (data, type, full) {
                    return data.termregistrations.schoolclasses.name + " - " + data.termregistrations.subClasses.name;
                }
            },
            {
                "data": "createDate", "autoWidth": true,
                render: function (data, type, row) {
                    var hours = new Date(data).getHours()
                    let ap = hours >= 12 ? 'pm' : 'am';
                    return data = data.toLocaleString('YYYY-MM-dd').slice(0, 19).replace('T', ' ') + ' ' + ap;
                }
            }
        ]
    });
}