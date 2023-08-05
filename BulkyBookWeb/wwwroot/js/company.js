var dataTable;

$(document).ready(function () {
    loadDataTable();
});

const loadDataTable = () => {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: "/Admin/Company/GetAll" 
        },
        columns: [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },

            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center" role="group">
                            <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary me-1">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onClick="deleteCompany('/Admin/Company/Delete/${data}')" class="btn btn-danger ms-1">
                                <i class="bi bi-trash-fill"></i>
                            </a>
                        </div>
                    `;
                }
            }
        ]
    });
};

const deleteCompany = (url) => {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log(url);
            $.ajax({
                url: url,
                type: 'POST',
                success: function (data) {
                    if (data.success) { 
                        dataTable.ajax.reload(); 
                        toastr.success(data.message);
                    }
                    else { 
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
};