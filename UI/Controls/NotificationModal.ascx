<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotificationModal.ascx.cs" Inherits="UI.Controls.NotificationModal" %>

<div id="notificationModal"
     style="display:none; position:fixed; inset:0; z-index:9999; align-items:center; justify-content:center; padding:1rem;">

    <div onclick="NotificationModal.close()"
         style="position:absolute; inset:0; background:rgba(0,0,0,0.45); backdrop-filter:blur(2px);">
    </div>

    <div id="nmPanel"
         style="position:relative; z-index:1; width:100%; max-width:440px;
                background:#ffffff; border-radius:20px;
                box-shadow:0 24px 60px rgba(0,0,0,0.18), 0 8px 20px rgba(0,0,0,0.10);
                overflow:hidden;
                transform:scale(0.93) translateY(12px); opacity:0;
                transition:transform 280ms cubic-bezier(0.34,1.56,0.64,1), opacity 220ms ease;">

        <div id="nmBar" style="height:4px; width:100%;"></div>

        <div style="padding:28px 28px 24px 28px;">

            <div style="display:flex; align-items:flex-start; gap:16px;">

                <div id="nmIcon"
                     style="flex-shrink:0; width:46px; height:46px; border-radius:14px;
                            display:flex; align-items:center; justify-content:center;">
                </div>

                <div style="flex:1; min-width:0; padding-top:2px;">
                    <h3 id="nmTitle"
                        style="margin:0 0 6px 0; font-size:15px; font-weight:600;
                               color:#111827; line-height:1.3; letter-spacing:-0.01em;">
                    </h3>
                    <p id="nmMessage"
                       style="margin:0; font-size:14px; color:#6b7280; line-height:1.6;">
                    </p>
                </div>

                <%-- El X solo se muestra en errores (sin botones) --%>
                <button id="nmCloseX"
                        type="button"
                        onclick="NotificationModal.close()"
                        style="display:none; flex-shrink:0; width:30px; height:30px; border-radius:8px;
                               border:none; background:transparent; cursor:pointer;
                               align-items:center; justify-content:center;
                               color:#9ca3af; transition:background 150ms, color 150ms; margin-top:-2px;"
                        onmouseenter="this.style.background='#f3f4f6'; this.style.color='#374151';"
                        onmouseleave="this.style.background='transparent'; this.style.color='#9ca3af';"
                        aria-label="Cerrar">
                    <svg width="14" height="14" fill="none" viewBox="0 0 24 24"
                         stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                        <path d="M6 18L18 6M6 6l12 12"/>
                    </svg>
                </button>
            </div>

            <%-- Footer: oculto por defecto, JS lo muestra segun el modo --%>
            <div id="nmFooter" style="display:none;">
                <div style="height:1px; background:#f3f4f6; margin:22px 0 20px 0;"></div>
                <div style="display:flex; justify-content:flex-end; gap:10px;">

                    <%-- Botón cancelar: solo en modo "confirm" --%>
                    <button id="nmBtnCancel"
                            type="button"
                            onclick="NotificationModal.close()"
                            style="display:none; padding:9px 18px; border:1.5px solid #e5e7eb;
                                   background:transparent; border-radius:10px; font-size:13px;
                                   font-weight:500; color:#6b7280; cursor:pointer; transition:background 150ms;"
                            onmouseenter="this.style.background='#f9fafb';"
                            onmouseleave="this.style.background='transparent';">
                        Cancelar
                    </button>

                    <%-- Botón principal: success, info y confirm --%>
                    <button id="nmBtn"
                            type="button"
                            onclick="NotificationModal.close()"
                            style="display:none; padding:9px 22px; border:none; border-radius:10px;
                                   font-size:13px; font-weight:500; color:#ffffff; cursor:pointer;
                                   transition:filter 150ms, transform 80ms; letter-spacing:0.01em;"
                            onmouseenter="this.style.filter='brightness(1.1)';"
                            onmouseleave="this.style.filter='brightness(1)';"
                            onmousedown="this.style.transform='scale(0.97)';"
                            onmouseup="this.style.transform='scale(1)';">
                        Aceptar
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>

<script>
    var NotificationModal = (function () {

        var THEMES = {
            error: {
                bar: '#f87171', iconBg: '#fef2f2', iconFg: '#ef4444', btnBg: '#ef4444',
                svg: '<svg width="22" height="22" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>'
            },
            success: {
                bar: '#34d399', iconBg: '#f0fdf4', iconFg: '#10b981', btnBg: '#10b981',
                svg: '<svg width="22" height="22" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>'
            },
            info: {
                bar: '#60a5fa', iconBg: '#eff6ff', iconFg: '#3b82f6', btnBg: '#3b82f6',
                svg: '<svg width="22" height="22" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>'
            }
        };

        // mode: 'error' | 'success' | 'info' | 'confirm'
        function open(opts) {
            opts = opts || {};
            var type = opts.type || 'info';
            var mode = opts.mode || type;   // hereda el type si no se pasa mode
            var title = opts.title || '';
            var message = opts.message || '';
            var btnText = opts.btnText || 'Aceptar';
            var onConfirm = opts.onConfirm || null;

            var modal = document.getElementById('notificationModal');
            var panel = document.getElementById('nmPanel');
            var bar = document.getElementById('nmBar');
            var icon = document.getElementById('nmIcon');
            var footer = document.getElementById('nmFooter');
            var btn = document.getElementById('nmBtn');
            var btnCancel = document.getElementById('nmBtnCancel');
            var closeX = document.getElementById('nmCloseX');

            if (!modal) { console.error('NotificationModal: #notificationModal no encontrado.'); return; }

            var t = THEMES[type] || THEMES.info;

            // Textos
            document.getElementById('nmTitle').textContent = title;
            document.getElementById('nmMessage').textContent = message;

            // Colores
            bar.style.background = t.bar;
            icon.style.background = t.iconBg;
            icon.style.color = t.iconFg;
            btn.style.background = t.btnBg;
            icon.innerHTML = t.svg;

            // ── Visibilidad de botones según modo ──────────────────────────────
            //  error   → solo X arriba, sin footer
            //  success → footer con 1 botón (Aceptar)
            //  info    → footer con 1 botón (Aceptar)
            //  confirm → footer con 2 botones (Cancelar + Aceptar)

            if (mode === 'error') {
                closeX.style.display = 'flex';
                footer.style.display = 'none';
                btn.style.display = 'none';
                btnCancel.style.display = 'none';
            } else if (mode === 'confirm') {
                closeX.style.display = 'none';
                footer.style.display = 'block';
                btn.style.display = 'inline-block';
                btnCancel.style.display = 'inline-block';
                btn.textContent = btnText;
                // callback de confirmación
                btn.onclick = function () {
                    NotificationModal.close();
                    if (typeof onConfirm === 'function') onConfirm();
                };
                btnCancel.onclick = function () { NotificationModal.close(); };
            } else {
                // success / info
                closeX.style.display = 'none';
                footer.style.display = 'block';
                btn.style.display = 'inline-block';
                btnCancel.style.display = 'none';
                btn.textContent = btnText;
                btn.onclick = function () { NotificationModal.close(); };
            }

            // Mostrar
            modal.style.display = 'flex';
            requestAnimationFrame(function () {
                panel.style.transform = 'scale(1) translateY(0)';
                panel.style.opacity = '1';
            });
        }

        function close() {
            var modal = document.getElementById('notificationModal');
            var panel = document.getElementById('nmPanel');
            if (!modal) return;
            panel.style.transform = 'scale(0.93) translateY(12px)';
            panel.style.opacity = '0';
            setTimeout(function () { modal.style.display = 'none'; }, 240);
        }

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') close();
        });

        return { open: open, close: close };
    })();
</script>
