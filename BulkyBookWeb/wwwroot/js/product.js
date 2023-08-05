var dataTable;

$(document).ready(function () {
    loadDataTable();
});

const loadDataTable = () => {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: "/Admin/Product/GetAll"
        },
        columns: [
            { "data": "title", "width": "22%" },
            { "data": "isbn", "width": "15%", "class": "d-none d-sm-table-cell" },
            { "data": "price", "width": "15%", "class": "text-end" },
            { "data": "author", "width": "15%", "class": "d-none d-md-table-cell" },
            { "data": "category", "width": "15%", "class": "d-none d-lg-table-cell" },
            {
                "data": "id",
                "render": function (data) { 
                    return `
                        <div class="text-center" role="group">
                            <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary me-1">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onClick="deleteProduct('/Admin/Product/Delete/${data}')" class="btn btn-danger ms-1">
                                <i class="bi bi-trash-fill"></i>
                            </a>
                        </div>
                    `;
                }
            }
        ]
    });
};

const deleteProduct = (url) => {
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