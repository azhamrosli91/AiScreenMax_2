$(document).ready(function () {
    loadSideBar();

    // Highlight and expand based on current URL
    highlightAndExpand();
    $(".active a").click(function (e) {
        e.stopPropagation();
    });

});


function loadSideBar() {
    var sidebar = JSON.parse(localStorage.getItem('Sidebar'));
    var html = '';
    var html_mini = '';
    var html_mobile = '';
    var count = 0;

    if (sidebar == null || sidebar == "") {
        alert("Failed to load menu bar.");
        location.href = "/Home/Index";
        return;
    }
    $.each(sidebar, function (index, parent) {
        count++;
        if (parent.LAYER == 1) {
            if (parent.ListChild && parent.ListChild.length > 0) {
                // Parent has children, create a dropdown
                           /* <div class="fa fa-chevron-down dropdown-toggle-icon" style="margin-top: 3px"></div>*/
                html += `<a href="#pageSubmenu_${count}" data-bs-toggle="collapse" data-bs-parent="#accordionExample11_${count}" aria-expanded="false" class="dropdown-toggle nav-link nav-header header-text collapsed" aria-controls="pageSubmenu_${count}">
                            <span class='fa ${parent.ICON}'></span>&nbsp;&nbsp;${parent.RESOURCE_NAME}
                            <div class="fa fa-chevron-right dropdown-toggle-icon" style="margin-top: 3px"></div>
                             <div class="fa fa-chevron-down dropdown-toggle-icon" style="margin-top: 3px"></div>
                        </a>
                        <ul class="list-unstyled collapse" id="pageSubmenu_${count}">`;


                // Render child items
                $.each(parent.ListChild, function (childIndex, child) {
                    html += `<li class="nav-item">
                                <a href="/${child.RESOURCE_CONTROLLER}/${child.RESOURCE_VIEW}" id="btnDashboard_${count}_${childIndex}" class="nav-link sub-header-text"  title="${parent.RESOURCE_NAME}"><span class='fa ${child.ICON}'></span>&nbsp;&nbsp;${child.RESOURCE_NAME}</a>
                            </li>`;
                    html_mini += `<a href="/${child.RESOURCE_CONTROLLER}/${child.RESOURCE_VIEW}" class="nav-link nav-header header-text" title="${parent.RESOURCE_NAME}">
                           <span class='fa ${child.ICON}'></span>
                        </a>`;

                    html_mobile += `<li class="nav-item"><a href="/${child.RESOURCE_CONTROLLER}/${child.RESOURCE_VIEW}" class="nav-link tr-navbar-logout" title="${parent.RESOURCE_NAME}">
                        ${child.RESOURCE_NAME}
                        </a ></li>`;
                });

                html += `</ul>`;
            } else {
                // Parent has no children, create a direct link
                html += `<a href="/${parent.RESOURCE_CONTROLLER}/${parent.RESOURCE_VIEW}" class="nav-link nav-header header-text" title="${parent.RESOURCE_NAME}">
                           <span class='fa ${parent.ICON}'></span>&nbsp;&nbsp;${parent.RESOURCE_NAME}
                        </a>`;

                html_mini += `<a href="/${parent.RESOURCE_CONTROLLER}/${parent.RESOURCE_VIEW}" class="nav-link nav-header header-text" title="${parent.RESOURCE_NAME}">
                           <span class='fa ${parent.ICON}' title='${parent.RESOURCE_NAME}'></span>
                        </a>`;
                html_mobile += `<li class="nav-item"><a href="/${parent.RESOURCE_CONTROLLER}/${parent.RESOURCE_VIEW}" class="nav-link tr-navbar-logout" title="${parent.RESOURCE_NAME}">
                           ${parent.RESOURCE_NAME}
                        </a></li>`;
            }
        }
    });

    $('#sidebar-list').html(html);
    $('#sidebar-list-mobile').html(html_mobile);
    $('#sidebar-list-mini').html(html_mini);
}

function highlightAndExpand() {
    var currentUrl = window.location.pathname;
    var foundMatch = false;

    // Highlight the current link and expand parent dropdown if necessary
    $('#sidebar-list a').each(function () {
        var linkUrl = $(this).attr('href');

        if (linkUrl === currentUrl) {
            foundMatch = true;
        
            // Highlight the current link
            $(this).addClass('active');

            // Check if it's inside a dropdown
            var parentSubMenu = $(this).closest('ul.collapse');
            if (parentSubMenu.length > 0) {
                // Expand the parent dropdown
                parentSubMenu.addClass('show');

                // Highlight the parent link
                var parentLink = parentSubMenu.prev('a');
                parentLink.addClass('active');
                parentLink.attr('aria-expanded', 'true');
            }
        }
    });

    // If no exact match is found, find a similar controller with action 'Index'
    if (!foundMatch) {
        var currentController = currentUrl.split('/')[1]; // Extract controller name

        $('#sidebar-list a').each(function () {
            var linkUrl = $(this).attr('href');
            var linkParts = linkUrl.split('/');

            // Match controller and 'Index' action
            if (linkParts[1] === currentController && linkParts[2] === 'Index') {
                foundMatch = true;

                // Highlight the similar link
                $(this).addClass('active');

                // Check if it's inside a dropdown
                var parentSubMenu = $(this).closest('ul.collapse');
                if (parentSubMenu.length > 0) {
                    // Expand the parent dropdown
                    parentSubMenu.addClass('show');

                    // Highlight the parent link
                    var parentLink = parentSubMenu.prev('a');
                    parentLink.addClass('active');
                    parentLink.attr('aria-expanded', 'true');
                }
            }
        });
    }

}

