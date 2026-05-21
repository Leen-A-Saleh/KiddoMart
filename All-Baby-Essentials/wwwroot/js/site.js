// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function () {

    // --- Cart Offcanvas Logic ---
    const cartOffcanvasEl = document.getElementById('cartOffcanvas');
    if (cartOffcanvasEl) {
        cartOffcanvasEl.addEventListener('show.bs.offcanvas', function () {
            loadCartOffcanvas();
        });
    }

    function loadCartOffcanvas() {
        $.get('/Customer/Cart/GetCartOffcanvas', function (data) {
            $('#cartOffcanvasBody').html(data);
        }).fail(function () {
            $('#cartOffcanvasBody').html('<div class="text-center text-danger py-4">Failed to load cart.</div>');
        });
    }

    // --- Wishlist Offcanvas Logic ---
    const wishlistOffcanvasEl = document.getElementById('wishlistOffcanvas');
    if (wishlistOffcanvasEl) {
        wishlistOffcanvasEl.addEventListener('show.bs.offcanvas', function () {
            loadWishlistOffcanvas();
        });
    }

    function loadWishlistOffcanvas() {
        $.get('/Customer/Wishlist/GetWishlistOffcanvas', function (data) {
            $('#wishlistOffcanvasBody').html(data);
        }).fail(function () {
            $('#wishlistOffcanvasBody').html('<div class="text-center text-danger py-4">Failed to load wishlist.</div>');
        });
    }

    // --- Add to Cart (AJAX) ---
    $(document).on('click', '.add-to-cart-btn', function (e) {
        e.preventDefault();
        const btn = $(this);
        const productId = btn.data('id');
        const originalHtml = btn.html();

        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>').prop('disabled', true);

        $.post('/Customer/Cart/AddToCartAjax', { productId: productId }, function (res) {
            if (res.success) {
                $('#cartCountBadge').text(res.count);
                btn.html('<i class="bi bi-check-lg"></i> Added');
                setTimeout(() => btn.html(originalHtml).prop('disabled', false), 2000);
                // Show elegant success toast
                showElegantToast('Added to Cart', 'Item was successfully added to your cart.', 'success');
                
                // Show Offcanvas automatically
                if (cartOffcanvasEl) {
                    var offcanvas = bootstrap.Offcanvas.getOrCreateInstance(cartOffcanvasEl);
                    offcanvas.show();
                }
            }
        }).fail(function () {
            btn.html(originalHtml).prop('disabled', false);
            alert('Failed to add to cart. Please try again or log in.');
        });
    });

    // --- Add to Wishlist (AJAX) ---
    $(document).on('click', '.add-to-wishlist-btn', function (e) {
        e.preventDefault();
        const btn = $(this);
        const productId = btn.data('id');
        const originalHtml = btn.html();

        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>').prop('disabled', true);

        $.post('/Customer/Wishlist/AddAjax', { productId: productId }, function (res) {
            if (res.success) {
                $('#wishlistBadgeCount').text(res.count); // If element exists
                // Keep the heart filled to indicate it's in the wishlist
                btn.html('<i class="bi bi-heart-fill" style="color: var(--km-accent); font-size: 1.2rem; transform: scale(1.1); transition: all 0.2s ease;"></i>');
                btn.prop('disabled', false); // re-enable button without reverting HTML

                if (wishlistOffcanvasEl) {
                    var offcanvas = bootstrap.Offcanvas.getOrCreateInstance(wishlistOffcanvasEl);
                    offcanvas.show();
                }
            }
        }).fail(function () {
            btn.html(originalHtml).prop('disabled', false);
            alert('Failed to add to wishlist.');
        });
    });

    // --- Remove from Cart ---
    $(document).on('click', '.remove-cart-item', function () {
        const productId = $(this).data('id');
        $.post('/Customer/Cart/RemoveAjax', { productId: productId }, function (res) {
            if (res.success) {
                $('#cartCountBadge').text(res.count);
                loadCartOffcanvas();
            }
        });
    });

    // --- Update Cart Quantity ---
    $(document).on('click', '.increase-qty', function () {
        const input = $(this).siblings('.update-qty');
        let val = parseInt(input.val());
        input.val(val + 1);
        updateCartQty(input.data('id'), val + 1);
    });

    $(document).on('click', '.decrease-qty', function () {
        const input = $(this).siblings('.update-qty');
        let val = parseInt(input.val());
        if (val > 1) {
            input.val(val - 1);
            updateCartQty(input.data('id'), val - 1);
        } else {
            // If it hits 0, remove the item
            $(this).closest('.km-cart-item').find('.remove-cart-item').click();
        }
    });

    $(document).on('change', '.update-qty', function () {
        let val = parseInt($(this).val());
        if (val < 1) val = 1;
        $(this).val(val);
        updateCartQty($(this).data('id'), val);
    });

    function updateCartQty(productId, qty) {
        $.post('/Customer/Cart/UpdateQuantityAjax', { productId: productId, quantity: qty }, function (res) {
            if (res.success) {
                $('#cartCountBadge').text(res.count);
                loadCartOffcanvas();
            }
        });
    }

    // --- Remove from Wishlist ---
    $(document).on('click', '.remove-wishlist-item', function () {
        const productId = $(this).data('id');
        $.post('/Customer/Wishlist/RemoveAjax', { productId: productId }, function (res) {
            if (res.success) {
                if ($('#wishlistBadgeCount').length) $('#wishlistBadgeCount').text(res.count);
                loadWishlistOffcanvas();
            }
        });
    });

    // --- Move Wishlist to Cart ---
    $(document).on('click', '.add-to-cart-from-wishlist', function () {
        const btn = $(this);
        const productId = btn.data('id');
        
        btn.prop('disabled', true).text('Moving...');
        
        // Add to cart
        $.post('/Customer/Cart/AddToCartAjax', { productId: productId }, function (resCart) {
            if (resCart.success) {
                $('#cartCountBadge').text(resCart.count);
                // Remove from wishlist
                $.post('/Customer/Wishlist/RemoveAjax', { productId: productId }, function (resWish) {
                    if (resWish.success) {
                        if ($('#wishlistBadgeCount').length) $('#wishlistBadgeCount').text(resWish.count);
                        loadWishlistOffcanvas();
                    }
                });
            }
        });
    });

    // --- Quick View Modal ---
    const quickViewModalEl = document.getElementById('quickViewModal');
    let quickViewModal = null;
    if (quickViewModalEl) {
        quickViewModal = new bootstrap.Modal(quickViewModalEl);
    }

    $(document).on('click', '.quick-view-btn', function (e) {
        e.preventDefault();
        const productId = $(this).data('id');
        
        $('#quickViewModalBody').html('<div class="text-center py-5"><div class="spinner-border text-primary" role="status"></div></div>');
        if (quickViewModal) quickViewModal.show();

        $.get('/Customer/Product/GetQuickView/' + productId, function (data) {
            $('#quickViewModalBody').html(data);
        }).fail(function () {
            $('#quickViewModalBody').html('<div class="text-center text-danger py-5">Failed to load product details.</div>');
        });
    });

    // --- Custom Elegant Toast Function ---
    function showElegantToast(title, message, type = 'success') {
        const toastId = 'toast-' + Math.random().toString(36).substr(2, 9);
        const icon = type === 'success' ? '<i class="bi bi-check-circle-fill" style="color: var(--km-accent); font-size: 1.5rem;"></i>' : '<i class="bi bi-info-circle-fill" style="color: var(--km-primary); font-size: 1.5rem;"></i>';
        
        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center border-0 shadow-lg" role="alert" aria-live="assertive" aria-atomic="true" style="background: var(--km-surface); border-radius: var(--km-radius-sm); border-left: 4px solid var(--km-accent);">
              <div class="d-flex">
                <div class="toast-body d-flex align-items-center gap-3 py-3 px-4">
                  ${icon}
                  <div>
                      <strong class="d-block text-dark" style="font-family: var(--km-font-brand); font-size: 1.05rem;">${title}</strong>
                      <span class="text-muted" style="font-size: 0.9rem;">${message}</span>
                  </div>
                </div>
                <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
              </div>
            </div>
        `;
        
        if ($('#toast-container').length === 0) {
            $('body').append('<div id="toast-container" class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 1080;"></div>');
        }
        
        $('#toast-container').append(toastHtml);
        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
        toast.show();
        
        toastElement.addEventListener('hidden.bs.toast', function () {
            $(this).remove();
        });
    }

});
