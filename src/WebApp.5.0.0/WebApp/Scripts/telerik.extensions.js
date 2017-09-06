/*
* @Copyright (c) 2011 John DeVight
* Permission is hereby granted, free of charge, to any person
* obtaining a copy of this software and associated documentation
* files (the "Software"), to deal in the Software without
* restriction, including without limitation the rights to use,
* copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following
* conditions:
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
* OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
* NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
* WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
* OTHER DEALINGS IN THE SOFTWARE.
*/

/// <summary>
/// Extend the Telerik Extensions for ASP.NET MVC.
/// </summary>
(function ($) {
    
    if ($.telerik != undefined) {
        // Was the tekerik.tabstrip.min.js added to the page by the telerik script registrar?
        if ($.telerik.tabstrip != undefined) {
            // Extend the tabstrip plugin.
            var tabstripExtensions = {
                /// <summary>
                /// Get a tab.
                /// </summary>
                /// <param type="json object" name="o">json object with either the text or the index of the tab.</param>
                /// <return>jQuery object of the tab [li.t-item]</return>
                /// <example>
                /// var tab = $('#MyTabStrip').data('tTabStrip').getTab({ text: 'Tab 2' })
                /// var tab = $('#MyTabStrip').data('tTabStrip').getTab({ index: 1 })
                /// </example>
                getTab: function (o) {
                    var tab = null;
                    if (o.text != null) {
                        var tab = $(this.element).find('.t-item').find("a:contains('" + o.text + "')").parent();
                    }
                    else if (o.index != null) {
                        tab = $($(this.element).find('.t-item')[o.index]);
                    }
                    return tab;
                },
                /// <summary>
                /// Get index of tab.
                /// </summary>
                /// <param type="string" name="t">text of a tab.</param>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').getTabIndex('Tab 2')
                /// </example>
                getTabIndex: function (t) {
                    var idx = 0;
                    $.each$($(this.element).find('.t-tabstrip-items').find('a'), function (i, a) {
                        if ($(a).text() == t) {
                            idx = i;
                            return false;
                        }
                    })
                    return idx;
                },
                /// <summary>
                /// Hide a tab.
                /// </summary>
                /// <param type="json object" name="o">json object with either the text or the index of the tab.</param>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').hideTab({ text: 'Tab 2' })
                /// $('#MyTabStrip').data('tTabStrip').hideTab({ index: 1 })
                /// </example>
                hideTab: function (o) {
                    var tab = this.getTab(o);
                    if (tab != null) {
                        tab.css('visibility', 'hidden');
                        tab.css('display', 'none');
                    }
                },
                /// <summary>
                /// Show a tab.
                /// </summary>
                /// <param type="json object" name="o">json object with either the text or the index of the tab.</param>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').showTab({ text: 'Tab 2' })
                /// $('#MyTabStrip').data('tTabStrip').showTab({ index: 1 })
                /// </example>
                showTab: function (o) {
                    var tab = this.getTab(o);
                    if (tab != null) {
                        tab.css('visibility', '');
                        tab.css('display', '');
                    }
                },
                /// <summary>
                /// Select a tab.
                /// </summary>
                /// <param type="json object" name="o">json object with either the text or the index of the tab.</param>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').selectTab({ text: 'Tab 2' })
                /// $('#MyTabStrip').data('tTabStrip').selectTab({ index: 1 })
                /// </example>
                selectTab: function (o) {
                    var tab = this.getTab(o);
                    if (tab != null) {
                        this.select(tab);
                    }
                },
                /// <summary>
                /// Change the text of a tab.
                /// </summary>
                /// <param type="json object" name="o">json object with either the text or the index of the tab and the newText for the tab.</param>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').setTabText({ text: 'Tab 2', newText: 'Second Tab' })
                /// $('#MyTabStrip').data('tTabStrip').setTabText({ index: 1, newText: 'Second Tab' })
                /// </example>
                setTabText: function (o) {
                    var tab = this.getTab(o);
                    if (tab != null) {
                        tab.find('a').text(o.newText);
                    }
                },
                /// <summary>
                /// Add a tab.
                /// </summary>
                /// <param type="json object" name="t">json object with text and html for the tab.</param>
                /// <example>
                /// $.ajax({
                ///     url: '/Home/GetTabThree/',
                ///     contentType: 'application/html; charset=utf-8',
                ///     type: 'GET',
                ///     dataType: 'html'
                /// })
                /// .success(function(result) {
                ///     $('#MyTabStrip').data('tTabStrip').addTab({ text: 'Tab Three', html: result });
                /// });
                /// </example>
                addTab: function (t) {
                    var tabstrip = $(this.element);
                    var tabstripitems = tabstrip.find('.t-tabstrip-items');
                    var cnt = tabstripitems.children().length;
                    var tabname = tabstrip.attr('id');

                    tabstripitems.append(
                        $('<li />')
                            .addClass('t-item')
                            .addClass('t-state-default')
                            .append(
                                $('<a />')
                                    .attr('href', '#' + tabname + '-' + (cnt + 1))
                                    .addClass('t-link')
                                    .text(t.text)
                            )
                        );

                    var $contentElement =
                        $('<div />')
                            .attr('id', tabname + '-' + (cnt + 1))
                            .addClass('t-content')
                            .append(t.html)

                    tabstrip.append($contentElement);

                    tabstrip.data('tTabStrip').$contentElements.push($contentElement[0]);
                },
                /// <summary>
                /// Remove a tab.
                /// </summary>
                /// <param type="json object" name="o">json object with either the text or the index of the tab.</param>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').removeTab({ text: 'Tab 2' })
                /// $('#MyTabStrip').data('tTabStrip').removeTab({ index: 1 })
                /// </example>
                removeTab: function (o) {
                    var tabstrip = $(this.element);
                    var tabname = tabstrip.attr('id');
                    var tabstripitems = tabstrip.find('.t-tabstrip-items');
                    var i = 0;

                    if (o.index == undefined || o.index == null) {
                        i = this.getTabIndex(o.text);
                    }

                    // There must be atleast two tabs to remove a tab.
                    if (tabstripitems.children().length > 1) {
                        var tab = this.getTab({ index: i });
                        // If the active tab is being removed, set another tab as active.
                        if (tab.hasClass('t-state-active') == true) {
                            var j = i == 0 ? 1 : (i - 1);
                            this.activateTab(this.getTab({ index: j }));
                        }
                        tab.remove();

                        // Remove the tab contents.
                        $(tabstrip.children()[i + 1]).remove();
                        tabstrip.data('tTabStrip').$contentElements.splice(i, 1);

                        // Rename the tab href.
                        $.each(tabstripitems.children(), function (idx, tab) {
                            $($(tab).children()[0]).attr('href', '#' + tabname + '-' + (idx + 1));
                        });

                        // Rename tab contents.
                        $.each(tabstrip.children(), function (idx, contentElement) {
                            if ($(contentElement).is('div')) {
                                $(contentElement).attr('id', tabname + '-' + idx);
                            }
                        })
                    }
                },
                /// <summary>
                /// Hide the contents for all tabs in the tabstrip.
                /// </summary>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').hideContent()
                /// </example>
                hideContent: function () {
                    this.activateTab = function (d) {
                        var f = d.parent().children().removeClass("t-state-active").addClass("t-state-default").index(d);
                        d.removeClass("t-state-default").addClass("t-state-active");
                    };
                    $(this.element).height(25);
                    $.each($(this.element).find('.t-content'), function (idx, content) {
                        $(content).css('display', 'none')
                    });
                },
                /// <summary>
                /// Show the contents for all tabs in the tabstrip.
                /// </summary>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').showContent()
                /// </example>
                showContent: function () {
                    this.activateTab = $.telerik.tabstrip.prototype.activateTab;
                    $(this.element).css('height', '');
                    var t = this.getTab({ index: $(this.element).find('li.t-state-active').index() });
                    this.activateTab(t);
                },
                /// <summary>
                /// Remove the contents for all tabs in the tabstrip.
                /// Once this is done, there is no way of restoring the 
                /// tab contents.
                /// </summary>
                /// <example>
                /// $('#MyTabStrip').data('tTabStrip').removeContent()
                /// </example>
                removeContent: function () {
                    $(this.element).find('div.t-content', '.t-tabstrip').remove();
                    $(this.element).height(25);
                }
            };

            // Add the extensions to the tabstrip plugin.
            $.extend(true, $.telerik.tabstrip.prototype, tabstripExtensions);
        }



        // Was the tekerik.grid.min.js added to the page by the telerik script registrar?
        if ($.telerik.grid != undefined) {
            // Extend the grid plugin.
            var gridExtensions = {
                /// <summary>
                /// Hide a column in the grid.
                /// </summary>
                /// <param type="int" name="i">Zero based index for the column.</param>
                /// <example>
                /// $('#MyGrid').data('tTabStrip').hideColumn(1);
                /// </example>
                hideColumn: function (i) {
                    var table = $(this.element).find('table');

                    if (table.find('thead tr th').length > i) {
                        $($(table).find('thead tr th')[i]).css('display', 'none');
                    }

                    var rows = $(table).find('tbody tr');
                    if (rows.length >= 1 && $(rows[0]).find('td').length > i) {
                        $.each(rows, function (idx, row) {
                            $($(row).find('td')[i]).css('display', 'none');
                        });
                    }
                },
                /// <summary>
                /// Show a column in the grid.
                /// </summary>
                /// <param type="int" name="i">Zero based index for the column.</param>
                /// <example>
                /// $('#MyGrid').data('tGrid').showColumn(1);
                /// </example>
                showColumn: function (i) {
                    var table = $(this.element).find('table');

                    if (table.find('thead tr th').length > i) {
                        $($(table).find('thead tr th')[i]).css('display', '');
                    }

                    var rows = $(table).find('tbody tr');
                    if (rows.length >= 1 && $(rows[0]).find('td').length > i) {
                        $.each(rows, function (idx, row) {
                            $($(row).find('td')[i]).css('display', '');
                        });
                    }
                },
                /// <summary>
                /// Enable client side sorting in the grid.
                /// </summary>
                /// <example>
                /// $('#MyGrid').data('tGrid').enableClientSort();
                /// </example>
                enableClientSort: function () {
                    var $grid = this;
                    var headerCells = $(this.element).find('table thead tr th');
                    $.each(headerCells, function (idx, cell) {
                        var $a = $('<a />')
                            .attr('class', 't-link t-state-default');
                        $a.text($(cell).text());
                        cell.innerHTML = $('<div>').append($a.clone()).html();
                    })
                    this.sort = function (h) {
                        var data = $grid.data;

                        var sort_by = function (field, reverse, primer) {
                            reverse = (reverse) ? -1 : 1;
                            return function (a, b) {
                                a = a[field];
                                b = b[field];

                                if (typeof (primer) != 'undefined') {
                                    a = primer(a);
                                    b = primer(b);
                                }

                                if (a < b) return reverse * -1;
                                if (a > b) return reverse * 1;
                                return 0;
                            }
                        }

                        if (h.length > 0) {
                            this.orderBy = h;
                            var cols = h.split('~');
                            $.each(cols, function (idx, col) {
                                var params = col.split('-');
                                data.sort(sort_by(params[0], params[1] == 'desc', function (a) { return a.toString().toUpperCase() }));
                            });
                            $grid.dataBind(data);
                        }
                    }
                },
                /// <summary>
                /// Show a cell as a textbox in the grid.
                /// </summary>
                /// <example>
                /// $('#MyGrid').data('tGrid').showCellAsTextBox({ row: 1, column: 2 });
                ///
                /// $('#MyGrid').data('tGrid').showCellAsTextBox({ 
                ///     row: 1, 
                ///     column: 2, 
                ///     textBoxId: 'textBox_1_2', 
                ///     onChange: function(textBox, rowData, oldValue, newValue) {
                ///         if (rowData.NotNegative == true) {
                ///             if (rowData.value < 0) {
                ///                 textBox.css('color', 'red');
                ///             }
                ///             else {
                ///                 textBox.css('color', 'black');
                ///             }
                ///         }
                ///     }, 
                ///     formatDisplay: function(textBox, rowData) {
                ///         if (rowData.NotNegative == true) {
                ///             if (rowData.value < 0) {
                ///                 textBox.css('color', 'red');
                ///             }
                ///         }
                ///     },
                ///     formatPersist: function(textBox, rowData) {
                ///         return parseInt(textBox.val());
                ///     }
                /// });
                /// </example>
                showCellAsTextBox: function (o) {
                    if (this._editableCells == undefined) {
                        this._editableCells = [];
                    }

                    if (o.textBoxId == undefined) {
                        o.textBoxId = $(this.element).attr('id') + '_' + o.row + '_' + o.column;
                    }

                    var $input = $('<input type="text" />')
                        .attr('id', o.textBoxId)
                        .css('text-align', 'right')
                        .css('width', '100px')
                        .attr('onblur', '$("#' + $(this.element).attr('id') + '").data("tGrid").textBox_OnCellBlur($("#' + o.textBoxId + '"));');
                    var table = $(this.element).find('table');
                    var rows = $(table).find('tbody tr');
                    rows[o.row].cells[o.column].innerHTML = $('<div>').append($input.clone()).html();

                    var textBox = $('#' + o.textBoxId);

                    if (o.formatDisplay != undefined) {
                        o.formatDisplay(textBox, this.data[o.row]);
                    }
                    else {
                        textBox.val(this.data[o.row][this.columns[o.column].member]);
                    }

                    this._editableCells.push(o);

                    return textBox;
                },
                textBox_OnCellBlur: function (textBox) {
                    var column = textBox.parent('td');
                    var row = $(column).parent('tr');

                    var grid = this;

                    $.each(this._editableCells, function (idx, editableCell) {
                        if (editableCell.row == row[0].sectionRowIndex && editableCell.column == column[0].cellIndex) {
                            var newValue = textBox.val();
                            var rowData = grid.data[row[0].sectionRowIndex];
                            if (editableCell.formatPersist != undefined) {
                                newValue = editableCell.formatPersist(textBox, rowData);
                            }
                            var oldValue = rowData[grid.columns[column[0].cellIndex].member];
                            if (oldValue != newValue) {
                                rowData[grid.columns[column[0].cellIndex].member] = newValue;
                                if (editableCell.onChange != undefined) {
                                    editableCell.onChange(textBox, rowData, oldValue, newValue);
                                }
                            }
                            return;
                        }
                    });
                }
            };

            // Add the extensions to the grid plugin.
            $.extend(true, $.telerik.grid.prototype, gridExtensions);
        }

        // Was the tekerik.treeview.min.js added to the page by the telerik script registrar?
        if ($.telerik.treeview != undefined) {
            // Extend the treeview plugin.
            var treeviewExtensions = {
                /// <summary>
                /// Add a context menu to the treeview.
                /// </summary>
                /// <param type="json object" name="o">
                /// json object with a function to determine whether the context menu should be displayed 
                /// for a node and a list of menu items for the context menu.
                /// </param>
                /// <example>
                ///     $('#MyTreeView').data('tTreeView').addContextMenu({
                /// 	    evaluateNode: function(treeview, node) {
                /// 		    var nodeValue = treeview.getItemValue(node);
                /// 		    return (nodeValue == 'editable');
                /// 	    },
                /// 	    menuItems: [{
                /// 		        text: 'Edit',
                /// 		        onclick: function(e) {
                ///                     alert('You Clicked ' + e.item.text() + ' for ' + e.treeview.getItemText(e.node) + ' with a value of ' + e.treeview.getItemValue(e.node));
                ///                 }
                /// 	        }, {
                ///                 seperator: true
                ///             }, {
                /// 		        text: 'Delete',
                /// 		        onclick: function(e) {
                ///                     alert('You Clicked ' + e.item.text() + ' for ' + e.treeview.getItemText(e.node) + ' with a value of ' + e.treeview.getItemValue(e.node));
                ///                 }
                /// 	        }]
                ///     });
                /// </example>
                addContextMenu: function (o) {
                    if (this._contextMenus == undefined) {
                        this._contextMenus = [];

                        // subscribe to the contextmenu event to show a contet menu
                        $('.t-in', this.element).live('contextmenu', function (e) {
                            var treeview = $(e.liveFired).data('tTreeView');

                            var span = $(this);

                            // prevent the browser context menu from opening
                            e.preventDefault();

                            // the node for which the 'contextmenu' event has fired
                            var $node = span.closest('.t-item');

                            // default "slide" effect settings
                            var fx = $.telerik.fx.slide.defaults();

                            // Identify which context menu to use.
                            $.each(treeview._contextMenus, function (mdx, menu) {
                                // Does this context menu apply to this node?
                                if (menu.evaluateNode(treeview, $node) == true) {
                                    var menuItems = '';
                                    $.each(menu.menuItems, function (idx, item) {
                                        if (item.separator != undefined && item.separator == true) {
                                            menuItems += '<li class="t-item"><hr/></li>';
                                        }
                                        else {
                                            menuItems += '<li class="t-item"><a href="#" class="t-link">' + item.text + '</a></li>';
                                        }
                                    });
                                    if (menuItems.length > 0) {
                                        // context menu definition - markup and event handling
                                        var $contextMenu =
                                            $('<div class="t-animation-container" id="contextMenu">' +
                                                '<ul class="t-widget t-group t-menu t-menu-vertical" style="display:none">' +
                                                    menuItems +
                                                '</ul>' +
                                            '</div>')
                                            .css( //positioning of the menu
                                            {
                                            position: 'absolute',
                                            left: e.pageX, // x coordinate of the mouse
                                            top: e.pageY   // y coordinate of the mouse
                                        })
                                            .appendTo(document.body)
                                            .find('.t-item') // select the menu items
                                            .mouseenter(function () {
                                                // hover effect
                                                span.addClass('t-state-hover');
                                            })
                                            .mouseleave(function () {
                                                // remove the hover effect
                                                span.removeClass('t-state-hover');
                                            })
                                            .click(function (e) {
                                                e.preventDefault();
                                                var li = $(this);
                                                // dispatch the click
                                                $.each(menu.menuItems, function (idx, item) {
                                                    if (item.text == li.text()) {
                                                        item.onclick({ item: li, treeview: treeview, node: $node });
                                                        return;
                                                    }
                                                });
                                            })
                                            .end();

                                        // show the menu with animation
                                        $.telerik.fx.play(fx, $contextMenu.find('.t-group'), { direction: 'bottom' });

                                        // handle globally the click event in order to hide the context menu
                                        $(document).click(function (e) {
                                            // hide the context menu and remove it from DOM
                                            $.telerik.fx.rewind(fx, $contextMenu.find('.t-group'), { direction: 'bottom' }, function () {
                                                $contextMenu.remove();
                                            });
                                        });
                                    }
                                    return;
                                }
                            });
                        });
                    }
                    this._contextMenus.push(o);
                }
            };

            // Add the extensions to the treeview plugin.
            $.extend(true, $.telerik.treeview.prototype, treeviewExtensions);
        }

        // Was the tekerik.window.min.js added to the page by the telerik script registrar?
        if ($.telerik.window != undefined) {
            // Extend the window plugin.
            var windowExtensions = {
                /// <summary>
                /// Set a new height for the window.
                /// </summary>
                /// <param type="int" name="h">New height for the window.</param>
                setHeight: function (h) {
                    $(this.element).find('.t-window-content').height(h);
                },
                /// <summary>
                /// Set a new width for the window.
                /// </summary>
                /// <param type="int" name="w">New width for the window.</param>
                setWidth: function (w) {
                    $(this.element).find('.t-window-content').width(w);
                },
                redirectUrl: function (url) {
                    alert(url);
                }
            };
       
            // Add the extensions to the window plugin.
            $.extend(true, $.telerik.window.prototype, windowExtensions);
        }
    }
})(jQuery);