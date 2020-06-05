var cart = {
    init: function () {
        cart.regEvents();
    },
    regEvents: function () {
        $('#btnContinue').on('click', function () {
            window.location.href = "/";
        });

        $('#btnUpdate').on('click', function () {
            var listProduct = $('.txtQuantity'); // get all product trong cart
            var cartList = [];
            $.each(listProduct, function (i, tag) {
                cartList.push({ // name same 100% thì mới mapping đc (tên properties class)
                    Quantity: $(tag).val(),
                    Product: {
                        ID: $(tag).data('id')
                    }
                });
            });

            $.ajax({
                url: '/Cart/Update',
                data: { cartModel: JSON.stringify(cartList) }, // name same 100%
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });

        $('#btnDeleteAll').on('click', function () {
            $.ajax({
                url: '/Cart/DeleteAll',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });

        $('.btn-delete').off('click').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                data: { id: $(this).data('id') },
                url: '/Cart/Delete',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });

        $('#btnPayment').on('click', function () {
            window.location.href = "/thanh-toan";
        });
    }
}
cart.init();