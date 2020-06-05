var product = {
    init: function () {
        product.registerEvents();
    },

    registerEvents: function () {
        $('.btn-images').off('click').on('click', function (e) { // a
            $('#imageList').html(''); // popup lên thì null thằng nào có nó sẽ display (xóa của mấy thằng CÓ images mình click trước đó)

            e.preventDefault();
            $('#imagesManange').modal('show');
            $('#hidProductID').val($(this).data('id')); // set lại value mỗi khi click
            product.loadImages(); // show images thằng mình click hiện tại
        });

        $('#btnChooImages').on('click', function (e) { // button
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                // child tag a thì đều click đc -> bất kể content là gì
                $('#imageList').append('<div style="float: left"><img src="' + url + '" width="100" /><a href="#" class="btn-delImage"><i class="fa fa-times"></i></a></div>');

                // e sẽ binding theo cái tag chứ ko phải tạo sẵn rồi dùng
                $('.btn-delImage').off('click').on('click', function (e) { // a
                    e.preventDefault();
                    $(this).parent().remove(); // xóa div
                });
            };
            finder.popup();
        });

        $('#btnSaveImages').on('click', function () { // button
            var images = []; // could nhiều
            var id = $('#hidProductID').val();
            $.each($('#imageList img'), function (i, item) { // get all child mà là tag img của imageList
                images.push($(item).prop('src'));
            })
            $.ajax({
                url: '/Admin/Product/SaveImages',
                type: 'POST',
                data: {
                    id: id,
                    images: JSON.stringify(images)
                },
                dataType: 'json',
                success: function (response) {
                    if (response.status) {
                        $('#imagesManange').modal('hide');
                        $('#imageList').html(''); // xóa
                        alert('Save thành công');
                    }
                }
            });
        });
    },

    loadImages: function () {
        $.ajax({
            url: '/Admin/Product/LoadImages',
            type: 'GET',
            data: {
                id: $('#hidProductID').val()
            },
            dataType: 'json',
            success: function (response) {
                var data = response.data; // list
                var html = '';
                $.each(data, function (i, item) {
                    html += '<div style="float: left"><img src="' + item + '" width="100" /><a href="#" class="btn-delImage"><i class="fa fa-times"></i></a></div>'
                });
                $('#imageList').html(html); // append bổ sung | html xóa rồi bổ sung

                $('.btn-delImage').off('click').on('click', function (e) {
                    e.preventDefault();
                    $(this).parent().remove();
                });
            }
        });
    }
}
product.init(); // cái box cho toàn bộ cái HTML đó chỉ lấy ra sài