var product = {
    init: function () {
        product.registerEvents();
    },
    registerEvents: function () {
        $('.btn-active').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/Product/ChangeStatus",
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    console.log(response);
                    if (response.Status == true) {
                        btn.text('Enable');
                    }
                    else {
                        btn.text('Disable');
                    }
                }
            });
        });
    }
}
product.init(); // phải chạy js