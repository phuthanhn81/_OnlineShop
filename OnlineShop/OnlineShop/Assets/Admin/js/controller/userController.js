var user = {
    init: function () {
        user.registerEvents();
    },
    registerEvents: function () {
        // off all event btn-active (binding nhiều lần -> gọi all) sau đó thằng nào đc ấn thì mới gọi
        $('.btn-active').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id'); // lấy value mà attribute của nó có chữ data và sau nó là id
            $.ajax({
                url: "/Admin/User/ChangeStatus",
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
user.init(); // phải chạy js