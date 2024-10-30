var classbtn = ""; var classfa = "";
var ap = "";
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
            "url": "/api/otherPayemrnt",
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
                    var name = data.termregistration.studentsData.surName + ' ' + (data?.termregistration.studentsData.otherName || '') + ' ' + data.termregistration.studentsData.firstName;
                    return name;
                }
            },
            { "data": "specialPay.amount", "autoWidth": true },
            { "data": "specialPay.otherPayTables.name", "autoWidth": true},
            { "data": "amount", "autoWidth": true },
            {
                "data": null, "render": function (data, type, full) {
                    if (data.amount == data.specialPay.amount) {
                        return `<span class="badge h4 bg-success" title="Fees completed">
                        <i class="fas fa-check-double"></i> Completed
                        </span>`;
                    } else {
                        return `<label title="Part Payment" class="badge h3 bg-warning"><i class="fas fa-times-circle"></i> Part Payment</label>`;
                    }
                }
            },
            {
                "data": null, "render": function (data, type, full) {
                    return data.termregistration.sessionYear.name;
                }
            },
            {
                "data": null, "render": function (data, type, full) {
                    return data.termregistration.term;
                }
            },
            {
                "data": null, "render": function (data, type, full) {
                    return data.termregistration.schoolclasses.name + " - " + data.termregistration.subClasses.name;
                }
            },
            {
                "data": "createdDate", "autoWidth": true,
                render: function (data, type, row) {
                    var hours = new Date(data).getHours()
                    let ap = hours >= 12 ? 'pm' : 'am';
                    return data = data.toLocaleString('YYYY-MM-dd').slice(0, 19).replace('T', ' ') + ' ' + ap;
                }
            }
        ]
    });
}