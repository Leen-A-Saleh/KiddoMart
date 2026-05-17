        //nifitication
        const notifPanel = document.getElementById('notifPanel');
        const notifOverlay = document.getElementById('notifOverlay');
        const notifList = document.getElementById('notifList');
        const badge = document.querySelector('.km-badge');
        const closeNotifBtn = document.getElementById('closeNotifBtn');
        const markAllReadBtn = document.getElementById('markAllReadBtn');

        function openNotifPanel() {
            notifPanel.classList.add('open');
            notifOverlay.classList.add('open');
            loadNotifications();
        }

        function closeNotifPanel() {
            notifPanel.classList.remove('open');
            notifOverlay.classList.remove('open');
        }

        async function loadNotifications() {
            const res = await fetch('/Admin/Notification/GetAll');
            const data = await res.json();

            if (badge) {
                badge.textContent = data.unreadCount > 0 ? data.unreadCount : '';
            }

            if (data.notifications.length === 0) {
                notifList.innerHTML = `
                    <div class="km-notif-empty">
                        <i class="fas fa-bell-slash"></i>
                        <p>No notifications yet</p>
                    </div>`;
                return;
            }

            notifList.innerHTML = data.notifications.map(n => `
                <div class="km-notif-item ${!n.isRead ? 'unread' : ''}">
                    <div class="km-notif-icon ${n.color}">
                        <i class="${n.icon}"></i>
                    </div>
                    <div class="km-notif-body">
                        <div class="km-notif-title">${n.title}</div>
                        <div class="km-notif-msg">${n.message}</div>
                        <div class="km-notif-time">${timeAgo(n.createdAt)}</div>
                    </div>
                    <button type="button"
                            class="km-notif-delete"
                            data-id="${n.id}">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            `).join('');

            document.querySelectorAll('.km-notif-delete').forEach(button => {
                button.addEventListener('click', function () {
                    const id = this.dataset.id;
                    deleteNotif(id);
                });
            });
        }

        async function markAllRead() {
            await fetch('/Admin/Notification/MarkAllRead', {
                method: 'POST'
            });

            loadNotifications();
        }

        async function deleteNotif(id) {
            await fetch(`/Admin/Notification/Delete?id=${id}`, {
                method: 'POST'
            });

            loadNotifications();
        }

        function timeAgo(dateStr) {
            const diff = Math.floor((new Date() - new Date(dateStr)) / 1000);

            if (diff < 60) return 'Just now';
            if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
            if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;

            return `${Math.floor(diff / 86400)}d ago`;
        }

        document.addEventListener('DOMContentLoaded', async function () {

            notifOverlay.addEventListener('click', closeNotifPanel);

            closeNotifBtn.addEventListener('click', closeNotifPanel);

            markAllReadBtn.addEventListener('click', markAllRead);

            const res = await fetch('/Admin/Notification/GetAll');
            const data = await res.json();

            if (badge && data.unreadCount > 0) {
                badge.textContent = data.unreadCount;
            }
        });