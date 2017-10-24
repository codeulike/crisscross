// CrissCross - alternative user interface for running SSRS reports
// Copyright (C) 2011-2017 Ian Finch
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

function CrissCrossClient() {

    this.reportPath = null;
    this.log = new Array();
    this.showLog = null;
    this.availableParameters = null;
    this.runningMode = false;
    
    this.crissCrossTourHandler = null;
    // 0 = adapt, 1 = always assume slow, 2 = always assume fast
    this.browserAdaptMode = 0;
    // in slow browser mode, these control the item limit that trips over to lightwight controls
    this.singlePickItemLimit = 300;
    this.multiPickItemLimit = 200;
    // thee are set by call to browserCheck()
    this.slowBrowser = false;
    this.ie7Hacks = false;
    
    


    this.initClient = function() {
        this.addLog("Initialising...");
        this.browserCheck();
        if (this.slowBrowser)
            $.fx.off = true;
        
        $(".runButton").button();
        $("#testButton").button();
        var self = this;
        if (this.runningMode) {
            this.addLog("runningMode is true so not rendering params");
            $(".runButton").hide();
            $("#changeParams").button();
            $("#changeParams").show();
            $("#changeParams").click(function() { self.changeParameters() });
            this.addLog("queuing up scroll for when window is loaded");
            window.setTimeout(function() {
                var pos = $("#paramSummaryWrapper").offset().top;
                self.addLog("scrolling to: " + pos);
                $("html, body", top.document).animate({ "scrollTop": pos }, 1000);
            }, 1000);
        }
        else {
            this.addLog("queuing up parameter fetch for when window is loaded");
            // TODO: load has already fired?
            $(window).load(function() {
                window.setTimeout(function() {
                    self.fetchMinimumParametersToDisplay();
                }, 1000);
            });
        }
    };

    this.fetchAvailableParameters = function() {
        var self = this;
        var d = {
            url: "Report.aspx/getAvailableParameters",
            data: '{ "path": "' + this.reportPath + '"}',
            success: function(res) {
                self.availableParameters = eval(res.d);
                self.addLog("getAvailableParameters returned " + res.d);
                self.renderAvailableParameters();
            }
        };
        this.callAspNetPageMethod(d);
    }

    this.renderAvailableParameters = function() {
        var html = this.viewParamChooser(this.availableParameters);
        $("#clientParamDiv #waitBlock").before(html);
        $("#addParam").button();
        var self = this;
        $("#paramChooser").change(function() {
            var chosen = $("#paramChooser").val();
            self.addLog("paramChoose change fired, val: " + chosen);
            if (chosen != 0) {
                $("#extendParam").slideUp("fast", function() {
                    self.fetchParam(chosen);
                });
            }
        });
        this.checkAvailableParametersRemaining();
    };

    this.removeAvailableParameter = function(paramName) {
        $("#paramChooser option[value='" + paramName + "']").remove();
        this.checkAvailableParametersRemaining();
    };

    this.checkAvailableParametersRemaining = function() {
        if ($("#paramChooser option").size() == 1) {
            $("#paramChooser").hide();
            $("#allParamsDisplayed").show();
        }
    }

    this.fetchParam = function(paramName) {
        this.addLog("fetch " + paramName);
        var self = this;
        var d = {
            url: "Report.aspx/getParameterInfo",
            data: '{ "path": "' + this.reportPath + '", "paramName": "' + paramName + '"}',
            success: function(res) {
                self.addLog("getParameterInfo returned data " + res.d);
                var p = eval(res.d);
                self.renderParameter(p);
                self.revealParameter(p);
            }
        };
        this.callAspNetPageMethod(d);

    };

    this.renderParameter = function(paramInfo, renderBeforeId) {
        this.addLog("renderParameter called with " + paramInfo.Name);
        if (!renderBeforeId)
            renderBeforeId = "extendParam";
        if (paramInfo.ParameterType == 1)
            this.renderParameterTextBox(paramInfo, renderBeforeId);
        if (paramInfo.ParameterType == 2)
            this.renderParameterDatePick(paramInfo, renderBeforeId);
        if (paramInfo.ParameterType == 3) {
            var itemCount = 0;
            if (paramInfo.ValidValues)
                itemCount = paramInfo.ValidValues.length;
            if (this.slowBrowser && itemCount > this.singlePickItemLimit)
                this.renderParameterSinglePickLight(paramInfo, renderBeforeId);
            else
                this.renderParameterSinglePickAlternative(paramInfo, renderBeforeId);
        }
        if (paramInfo.ParameterType == 4) {
            var itemCount = 0;
            if (paramInfo.ValidValues)
                itemCount = paramInfo.ValidValues.length;
            if (this.slowBrowser && itemCount > this.multiPickItemLimit)
                this.renderParameterMultiPickLarge(paramInfo, renderBeforeId);
            else
                this.renderParameterMultiPick(paramInfo, renderBeforeId);
        }
        if (paramInfo.ParameterType == 5)
            this.renderParameterCheckBox(paramInfo, renderBeforeId);
        var blockElem = $("#" + paramInfo.id).get(0);
        blockElem.crissCrossName = paramInfo.Name;
        blockElem.crissCrossDisplayName = paramInfo.DisplayName;
    };

    this.revealParameter = function(paramInfo) {
        this.removeAvailableParameter(paramInfo.Name);
        if (this.ie7Hacks)
            $("#" + paramInfo.id).slideDown("fast", function() {
                $("#extendParam").slideDown("fast");
                $(".runButton").addClass("ie7hack");
                $(".runButton").removeClass("ie7hack");
            });
        else
            $("#" + paramInfo.id).slideDown("fast", function() {
                $("#extendParam").slideDown("fast");
            });
    };

    // renders a multi-pick
    // in general, the following get added to each paramBlock:
    //   crissCrossValue = fn returning array of values or single value
    //   crissCrossType = type of block (MultiPick, etc) used by help/tour system
    //   crissCrossDependants = array of ids of dependant parameters
    //   crissCrossChanged = flag used internally by some params to track clean/dirty state
    this.renderParameterMultiPick = function(paramInfo, renderBeforeId) {
        // html will be adjusted based on choice if there is one
        var html = this.viewMultiPick(paramInfo, "multiPick");
        $("#" + renderBeforeId).before(html);

        $("#" + paramInfo.id + "_control").multiselect({
            selectedList: 4,
            height: 400
        });
        if (paramInfo.AllowListSearch) {
            $("#" + paramInfo.id + "_control").multiselect().multiselectfilter({
                label: "Search",
                width: 50
            });
        }

        var blockElem = $("#" + paramInfo.id).get(0);
        blockElem.crissCrossValue = function() {
            var checkedValues = $.map($(this).find("select").multiselect("getChecked"), function(input) {
                return input.value;
            });
            return checkedValues;
        };
        blockElem.crissCrossType = 'MultiPick';
        // if emptyequivalent specified, wire up events to select it intelligently
        if (paramInfo.EmptyEquivalentValues.length > 0) {
            var firstEmptyEquivalent = paramInfo.EmptyEquivalentValues[0];
            $("#" + paramInfo.id + "_control").bind("multiselectclick", function(event, ui) {
                // nb: this gets called recursively when it makes changes to checkboxes
                if (ui.value == firstEmptyEquivalent) {
                    if (ui.checked) {
                        // ee selected, ensure nothing else is
                        $("#" + paramInfo.id + "_control").multiselect("widget").find(":checkbox:checked").each(function() {
                            if (this.value != firstEmptyEquivalent)
                                this.click();
                        });
                    }
                }
                else {
                    if (ui.checked) {
                        // turn off ee if it is selected
                        $("#" + paramInfo.id + "_control").multiselect("widget").find(":checkbox:checked[value='" + firstEmptyEquivalent + "']").each(function() {
                            this.click();
                        });
                    }
                }
            });
            $("#" + paramInfo.id + "_control").bind("multiselectcheckall", function() {
                // turn off ee if it is selected
                $("#" + paramInfo.id + "_control").multiselect("widget").find(":checkbox:checked[value='" + firstEmptyEquivalent + "']").each(function() {
                    this.click();
                });
            });
        }

        // if dependant parameters exist
        if (paramInfo.DependantParameterIds.length > 0) {
            var self = this;
            this.addLog("param " + paramInfo.id + " has dependants " + paramInfo.DependantParameterIds);
            blockElem.crissCrossDependants = paramInfo.DependantParameterIds;
            $("#" + paramInfo.id + "_control").bind("multiselectopen", function() {
                blockElem.crissCrossChanged = false;
            });
            $("#" + paramInfo.id + "_control").change(function() {
                blockElem.crissCrossChanged = true;
            });
            $("#" + paramInfo.id + "_control").bind("multiselectclose", function() {
                if (blockElem.crissCrossChanged)
                    self.refreshDependants(paramInfo.id);
            });
        }
    };

    // not used at the moment, had problems with dependedent parameters
    /*
    this.renderParameterMultiPickLight = function(paramInfo, renderBeforeId) {
        // html will be adjusted based on choice if there is one
        var html = this.viewMultiPick(paramInfo, "multiPickLight");
        $("#" + renderBeforeId).before(html);
        var blockElem = $("#" + paramInfo.id).get(0);
        var firstEmptyEquivalent = null;
        if (paramInfo.EmptyEquivalentValues.length > 0)
            firstEmptyEquivalent = paramInfo.EmptyEquivalentValues[0];
        var self = this;
        $("#" + paramInfo.id + "_control").selectList({
            onAdd: function(select, value, text) {
                blockElem.crissCrossChanged = true;
                if (firstEmptyEquivalent) {
                    var allSelected = $(select).val();
                    if (allSelected) {
                        // get selectlist api
                        var slapi = $(select).selectList({ instance: true });

                        if (value == firstEmptyEquivalent) {
                            // empty equivalent selected, so we want to remove all others
                            self.addLog("emptyEquiv " + value + " selected, others: " + allSelected);
                            for (i = 0; i < allSelected.length; i++) {
                                if (allSelected[i] == firstEmptyEquivalent)
                                    self.addLog("emptyEquiv " + firstEmptyEquivalent + " found in selected list");
                                else {
                                    slapi.remove(allSelected[i]);
                                }
                            }
                        }
                        else {
                            // normal value selected, so we want to remove empty equivalent
                            self.addLog("normal value " + value + " selected, others: " + allSelected);
                            for (i = 0; i < allSelected.length; i++) {
                                if (allSelected[i] == firstEmptyEquivalent)
                                    slapi.remove(allSelected[i]);
                            }
                        }
                    }
                }

            },
            onRemove: function(select, value, text) {
                blockElem.crissCrossChanged = true;
            }

        });


        blockElem.crissCrossValue = function() {
            var checkedValues = $(this).find("select").val();
            return checkedValues;
        };
        blockElem.crissCrossType = 'MultiPickLight';
        blockElem.crissCrossChanged = false;

        // if dependant parameters exist
        if (paramInfo.DependantParameterIds.length > 0) {
            var self = this;
            this.addLog("param " + paramInfo.id + " has dependants " + paramInfo.DependantParameterIds);
            blockElem.crissCrossDependants = paramInfo.DependantParameterIds;
            // get selectlist widget
            var slwidget = $("#" + paramInfo.id + "_control").next("select");
            slwidget.bind('blur', function() {
                self.addLog("param " + paramInfo.id + " blur ");
                if (blockElem.crissCrossChanged) {
                    blockElem.crissCrossChanged = false;
                    self.refreshDependants(paramInfo.id);
                }
            });
        }
    }
    */


    this.renderParameterMultiPickLarge = function(paramInfo, renderBeforeId) {

        var html = this.viewMultiPickLarge(paramInfo);
        $("#" + renderBeforeId).before(html);
        var blockElem = $("#" + paramInfo.id).get(0);

        // figure out any pre-selected values
        var selectedValues = null;
        if (paramInfo.ParameterChoice)
            selectedValues = paramInfo.ParameterChoice.Values;

        // make item collection suitable for tokeninput widget
        // also compile selected tokens
        var tokenList = new Array();
        var selectedTokens = new Array();
        for (var i = 0; i < paramInfo.ValidValues.length; i++) {
            tokenList.push({
                id: paramInfo.ValidValues[i].Value,
                name: paramInfo.ValidValues[i].Label
            });
            if (selectedValues && $.inArray(paramInfo.ValidValues[i].Value, selectedValues) > -1) {
                selectedTokens.push({
                    id: paramInfo.ValidValues[i].Value,
                    name: paramInfo.ValidValues[i].Label
                });
            }
        }


        var controlId = paramInfo.id + "_control";
        var firstEmptyEquivalent = null;
        if (paramInfo.EmptyEquivalentValues.length > 0)
            firstEmptyEquivalent = paramInfo.EmptyEquivalentValues[0];
        $("#" + controlId).tokenInput(tokenList,
            {
                theme: "crisscross",
                prePopulate: selectedTokens,
                onAdd: function(item) {
                    if (firstEmptyEquivalent) {
                        var selectedTokens = $("#" + controlId).tokenInput("get");
                        if (item.id == firstEmptyEquivalent) {
                            // remove all others
                            for (var i = 0; i < selectedTokens.length; i++)
                                if (selectedTokens[i].id != firstEmptyEquivalent)
                                $("#" + controlId).tokenInput("remove", { id: selectedTokens[i].id });
                        }
                        else {
                            // remove ee if its there
                            for (var i = 0; i < selectedTokens.length; i++)
                                if (selectedTokens[i].id == firstEmptyEquivalent)
                                $("#" + controlId).tokenInput("remove", { id: selectedTokens[i].id });

                        }
                    }
                }
            });


        blockElem.crissCrossValue = function() {
            var selectedTokens = $("#" + controlId).tokenInput("get");
            var selectedValues = $.map(selectedTokens, function(input) {
                return input.id;
            });
            return selectedValues;
        };
        blockElem.crissCrossType = 'MultiPickLarge';

        // wire up dialog event
        var self = this;
        $("#" + paramInfo.id + "_dialogLink").click(function(e) {
            e.preventDefault();
            // clone paramInfo
            var subParamInfo = $.extend(true, {}, paramInfo);
            subParamInfo.id = subParamInfo.id + "_dialog";
            // add current selection
            var parentBlockElem = $("#" + paramInfo.id).get(0);
            if (!subParamInfo.ParameterChoice)
                subParamInfo.ParameterChoice = {};
            subParamInfo.ParameterChoice.Values = parentBlockElem.crissCrossValue();

            self.showMultiPickDialog(subParamInfo, paramInfo.id + "_control");
        });

    }

    this.showMultiPickDialog = function(paramInfo, parentControl) {
        this.addLog("showMultiPickDialog: " + paramInfo.id + " ... parentControl: " + parentControl);
        var self = this;
        $("#multiPickDialog").append("<div class=\"multiPickDialog\"><div id=\"dialogWait\">Please Wait...</div></div>");
        $("#multiPickDialog").dialog({
            title: "Choose " + paramInfo.DisplayName,
            modal: true,
            width: 720,
            height: 420,
            draggable: false,
            resizable: true,
            open: function(event, ui) {
                self.addLog("dialog open event " + paramInfo.id);
                setTimeout(function() { self.addControlToMultiPickDialog(paramInfo, parentControl); }, 1000);
            },
            close: function(event, ui) {
                $(this).dialog('destroy');
                $("#multiPickDialog").empty();
            },
            buttons: {
                "Ok": function() {
                    var originalSelect = $("#" + paramInfo.id + "_control")[0];
                    var selectedItems = $(originalSelect).val();
                    self.setMultiPickLargeSelection(parentControl, selectedItems, paramInfo.ValidValues);
                    $(this).dialog("close");
                },
                "Cancel": function() { $(this).dialog("close"); }
            }
        });
    };

    this.addControlToMultiPickDialog = function(paramInfo, parentControl) {
        this.addLog("in addControlToMultiPickDialog");
        var selectHtml = "<div id=\"" + paramInfo.id + "\" style=\"display:none;\">"
                        + this.viewSelect(paramInfo, true) + "</div>";
        $("#dialogWait").before(selectHtml);
        $("#" + paramInfo.id + "_control").oxbowstilt(
            { listWidth: 310,
                height: 280
            });
        $("#" + paramInfo.id).show();
        $("#dialogWait").hide();
    };

    this.setMultiPickLargeSelection = function(controlId, selectedValues, validValues) {
        this.addLog("Setting " + controlId + " to " + selectedValues);
        // work out which tokens we want
        var selectedTokens = new Array();
        for (var i = 0; i < validValues.length; i++) {
            if (selectedValues && $.inArray(validValues[i].Value, selectedValues) > -1) {
                selectedTokens.push({
                    id: validValues[i].Value,
                    name: validValues[i].Label
                });
            }
        }
        // apply to control
        $("#"+controlId).tokenInput("clear");
        for (var i = 0; i < selectedTokens.length; i++)
            $("#"+controlId).tokenInput("add", selectedTokens[i]);
    }

    // use alternative below (with tweaks to multiselect) until chrome bug fixes
    /*
    this.renderParameterSinglePick = function(paramInfo, renderBeforeId) {
        // html will be adjusted based on choice if there is one
        var html = this.viewSinglePick(paramInfo, "singlePick");
        $("#" + renderBeforeId).before(html);
        $("#" + paramInfo.id + "_control").multiselect({
            multiple: false,
            selectedList: 1,
            height: 300
        });
        if (paramInfo.AllowListSearch) {
            $("#" + paramInfo.id + "_control").multiselect().multiselectfilter({
                label: "Search",
                width: 50
            });
        }
        var blockElem = $("#" + paramInfo.id).get(0);
        blockElem.crissCrossValue = function() {
            var checkedValues = $.map($(this).find("select").multiselect("getChecked"), function(input) {
                return input.value;
            });
            return checkedValues;
        };
        blockElem.crissCrossType = 'SinglePick';
        // if dependant parameters exist
        if (paramInfo.DependantParameterIds.length > 0) {
            var self = this;
            this.addLog("param " + paramInfo.id + " has dependants " + paramInfo.DependantParameterIds);
            blockElem.crissCrossDependants = paramInfo.DependantParameterIds;
//            $("#" + paramInfo.id + "_control").bind("multiselectclick", function(event, ui) {
//                if (ui.checked)
//                    self.refreshDependants(paramInfo.id);
//            });
            $("#" + paramInfo.id + "_control").change(function() {
                self.refreshDependants(paramInfo.id);
            });
        }

    };
    */

    // single pick using multiselect that avoids the chrome radiobutton bug
    this.renderParameterSinglePickAlternative = function(paramInfo, renderBeforeId) {
        // html will be adjusted based on choice if there is one
        var html = this.viewSinglePick(paramInfo, "singlePick");
        $("#" + renderBeforeId).before(html);
        $("#" + paramInfo.id + "_control").multiselect({
            multiple: false,
            selectedList: 1,
            height: 300
        });
        if (paramInfo.AllowListSearch) {
            $("#" + paramInfo.id + "_control").multiselect().multiselectfilter({
                label: "Search",
                width: 50
            });
        }
        var blockElem = $("#" + paramInfo.id).get(0);
        blockElem.crissCrossValue = function() {
            var checkedValues = $.map($(this).find("select").multiselect("getChecked"), function(input) {
                return input.value;
            });
            return checkedValues;
        };
        blockElem.crissCrossType = 'SinglePick';
        // if dependant parameters exist
        if (paramInfo.DependantParameterIds.length > 0) {
            var self = this;
            this.addLog("param " + paramInfo.id + " has dependants " + paramInfo.DependantParameterIds);
            blockElem.crissCrossDependants = paramInfo.DependantParameterIds;
            //            $("#" + paramInfo.id + "_control").bind("multiselectclick", function(event, ui) {
            //                if (ui.checked)
            //                    self.refreshDependants(paramInfo.id);
            //            });
            $("#" + paramInfo.id + "_control").change(function() {
                self.refreshDependants(paramInfo.id);
            });
        }

        // ensure only 1 checked
        var self = this;
        $("#" + paramInfo.id + "_control").bind("multiselectclick", function(event, ui) {

            var justChecked = ui.value;
            // make sure only 1 selected
            $("#" + paramInfo.id + "_control").multiselect("widget").find(":checkbox:checked").each(function() {
                if (this.value != justChecked) {
                    self.addLog("singleSelectAlternative " + justChecked + " selected so removing " + this.value);
                    this.click();
                }
            });
        });

   
    };

    this.renderParameterSinglePickLight = function(paramInfo, renderBeforeId) {
        // html will be adjusted based on choice if there is one
        var html = this.viewSinglePick(paramInfo, "singlePickLight");
        $("#" + renderBeforeId).before(html);

        var blockElem = $("#" + paramInfo.id).get(0);
        blockElem.crissCrossValue = function() {
            return $(this).find("select").val();
        };
        blockElem.crissCrossType = 'SinglePickLight';
        // if dependant parameters exist
        if (paramInfo.DependantParameterIds.length > 0) {
            var self = this;
            this.addLog("param " + paramInfo.id + " has dependants " + paramInfo.DependantParameterIds);
            blockElem.crissCrossDependants = paramInfo.DependantParameterIds;
            $("#" + paramInfo.id + "_control").change(function() {
                self.refreshDependants(paramInfo.id);
            });
        }
    };

    this.renderParameterDatePick = function(paramInfo, renderBeforeId) {
        var html = this.viewDatePick(paramInfo);
        $("#" + renderBeforeId).before(html);
        $("#" + paramInfo.id + "_control").datepicker({
            showOn: "both",
            buttonImage: "Content/images/calendar.gif",
            buttonImageOnly: true,
            dateFormat: 'dd/mm/yy',
            showOtherMonths: true,
            selectOtherMonths: true
        });
        var blockElem = $("#" + paramInfo.id).get(0);
        blockElem.crissCrossValue = function() {
            var val = $(this).find("input").datepicker("getDate");
            if (val)
                val = $.datepicker.formatDate('yy-mm-dd', val);
            return val;
        }
        blockElem.crissCrossType = 'DatePick';
        // set value if there is a value
        if (paramInfo.ParameterChoice) {
            var datevalstring = paramInfo.ParameterChoice.SingleValue;
            if (datevalstring) {
                var dateval = this.utcStringToDate(datevalstring)
                this.addLog("setting param " + paramInfo.Name + " to " + datevalstring + " which is " + dateval);
                $("#" + paramInfo.id + "_control").datepicker("setDate", dateval);
            }
        }

        // if dependant parameters exist
        if (paramInfo.DependantParameterIds.length > 0) {
            var self = this;
            this.addLog("param " + paramInfo.id + " has dependants " + paramInfo.DependantParameterIds);
            blockElem.crissCrossDependants = paramInfo.DependantParameterIds;
            $("#" + paramInfo.id + "_control").change(function() {
                self.refreshDependants(paramInfo.id);
            });
        };


    };

    this.renderParameterCheckBox = function(paramInfo, renderBeforeId) {
        // html will be adjusted based on choice if there is one
        var html = this.viewCheckBox(paramInfo);
        $("#" + renderBeforeId).before(html);
        var blockElem = $("#" + paramInfo.id).get(0);
        blockElem.crissCrossValue = function() {
            var chked = $(this).find("input").is(':checked');
            if (chked)
                return "True";
            else
                return "False";
        }
        blockElem.crissCrossType = 'CheckBox';
        // if dependant parameters exist
        if (paramInfo.DependantParameterIds.length > 0) {
            var self = this;
            this.addLog("param " + paramInfo.id + " has dependants " + paramInfo.DependantParameterIds);
            blockElem.crissCrossDependants = paramInfo.DependantParameterIds;
            $("#" + paramInfo.id + "_control").change(function() {
                self.refreshDependants(paramInfo.id);
            });
        };
    }

    this.renderParameterTextBox = function(paramInfo, renderBeforeId) {
        var html = this.viewTextBox(paramInfo);
        $("#" + renderBeforeId).before(html);
        var blockElem = $("#" + paramInfo.id).get(0);
        blockElem.crissCrossValue = function() {
            var val = $(this).find("input").val();
            return val;
        };
        blockElem.crissCrossType = 'TextBox';
        // set value if we have it
        if (paramInfo.ParameterChoice) {
            var valstring = paramInfo.ParameterChoice.SingleValue;
            $("#" + paramInfo.id + "_control").val(valstring);
        };

        // if dependant parameters exist
        if (paramInfo.DependantParameterIds.length > 0) {
            var self = this;
            this.addLog("param " + paramInfo.id + " has dependants " + paramInfo.DependantParameterIds);
            blockElem.crissCrossDependants = paramInfo.DependantParameterIds;
            $("#" + paramInfo.id + "_control").change(function() {
                self.refreshDependants(paramInfo.id);
            });
        };
    };

    this.utcStringToDate = function(utcString) {
        utcString = utcString.toString();
        var year = parseInt(utcString.substr(0, 4), 10);
        var month = parseInt(utcString.substr(5, 2), 10);
        var day = parseInt(utcString.substr(8, 2), 10);
        return new Date(year, month - 1, day);
    };

    this.refreshDependants = function(paramSourceName) {
        this.addLog("refreshDependants called for " + paramSourceName);
        var blockElem = $("#" + paramSourceName).get(0);
        var self = this;
        if (!blockElem.crissCrossDependants) {
            this.addLog("no dependants defined for " + paramSourceName);
            return;
        }

        var pname = blockElem.crissCrossName;
        var visibleDependants = new Array();
        $.each(blockElem.crissCrossDependants, function(index, paramInfoName) {
            self.addLog("checking if visible ... " + paramInfoName);
            if ($("#" + paramInfoName).length) {
                var dname = $("#" + paramInfoName).get(0).crissCrossName;
                visibleDependants.push(dname);
                var replaceBlockName = paramInfoName + "_replace";
                $("#" + paramInfoName).replaceWith(self.viewParameterReplacementBlock(replaceBlockName));    
            }
        });
        
        this.addLog("visible dependants: " + visibleDependants.length);
        
        var pval = blockElem.crissCrossValue();
        var paramStringObject = new Object();
        this.addToParamString(paramStringObject, pname, pval);
        this.addLog("value of " + paramSourceName + " = " + paramStringObject.val);

        this.addLog("calling pagemethod to get changes ....");
        var senddata = { path: this.reportPath,
            paramName: pname,
            paramValue: paramStringObject.val,
            visibleDependants: visibleDependants
        };
        var senddataJson = JSON.stringify(senddata);
        this.addLog("json: " + senddataJson);
        var d = {
            url: "Report.aspx/getDependantParameters",
            data: senddataJson,
            success: function(res) {
                //self.addLog("getDependantParameters returned data " + res.d);
                self.renderDependants(eval(res.d));
            }
        };
        this.callAspNetPageMethod(d);



    };

    this.renderDependants = function(paramList) {
        this.addLog("renderDependants called");
        var self = this;
        $.each(paramList, function(index, paramInfo) {
            self.addLog("refreshing param " + paramInfo.Name);
            var replaceBlockName = paramInfo.id + "_replace";
            self.renderParameter(paramInfo, replaceBlockName);
            $("#" + paramInfo.id).show();
            $("#" + replaceBlockName).remove();
            $("#" + paramInfo.id).effect("highlight");

        });
    }

    this.changeParameters = function() {
        var self = this;
        $("#clientParamDiv").slideDown(function() {
            self.fetchMinimumParametersToDisplay();
            $(".runButton").slideDown();
        });

        $("#changeParams").slideUp();
    }

    this.fetchMinimumParametersToDisplay = function() {
        this.addLog("fetchMinimumParametersToDisplay called");
        var self = this;
        var d = {
        url: "Report.aspx/getMinimumParametersToDisplay",
            data: '{ "path": "' + this.reportPath + '"}',
            success: function(res) {
            self.addLog("getMinimumParametersToDisplay returned data " + res.d);
                self.renderParameterListAndAvailableParameters(eval(res.d));
            }
        };
        this.callAspNetPageMethod(d);
    };

    this.renderParameterListAndAvailableParameters = function(paramInfoAndAvailableList) {
        this.addLog("renderParameterListAndAvailableParameters called");

        this.availableParameters = paramInfoAndAvailableList.AvailableParameters;
        this.addLog("getAvailableParameters returned " + this.availableParameters);
        this.renderAvailableParameters();
        this.renderParameterList(paramInfoAndAvailableList.ParameterDefinitions);
        if (this.crissCrossTourHandler)
            this.crissCrossTourHandler.createReportPageTour();
    };

    this.renderParameterList = function(paramInfoList) {
        var self = this;
        $.each(paramInfoList, function(index, paramInfo) {
            self.renderParameter(paramInfo);
            self.revealParameter(paramInfo);
        });
    }
    

    this.viewParamChooser = function(avail) {
        var html = "<div id=\"extendParam\" class=\"extendParamBlock\"><select id=\"paramChooser\">";
        html += "<option value=\"0\">&lt;Choose a filter&gt;</option>";
        $.each(avail, function(index, param) {
            html = html + "<option value=\"" + param.Key + "\">" + param.Value + "</option>";
        });
        html += "</select>";
        html += "<span id=\"allParamsDisplayed\" style=\"display:none;\">(All filters displayed)</span></div>";
        return html;
    };

    this.viewParameterGeneral = function(paramInfo, innerControlHtml) {
        var html = "<div id=\"" + paramInfo.id + "\" class=\"paramBlock\" style=\"display:none\"><div class=\"paramName\">" + paramInfo.DisplayName + "</div>";
        html += "<div class=\"paramControl\">" + innerControlHtml + "</div>";
        html += "<div class=\"clear\"></div></div>";
        return html;
    };

    this.viewParameterReplacementBlock = function(blockid) {
        var html = "<div id=\"" + blockid + "\" class=\"paramBlock\" ><div class=\"paramName\">Refreshing ... </div>";
        html += "<div class=\"paramControl\">(Please wait)</div>";
        html += "<div class=\"clear\"></div></div>";
        return html;
    }

    this.viewMultiPick = function(paramInfo, cssclass) {
        
        var htmlControl = this.viewSelect(paramInfo, true, cssclass);
        var html = this.viewParameterGeneral(paramInfo, htmlControl);
        
        return html;
    };

    this.viewSinglePick = function(paramInfo, cssclass) {
        var htmlControl = this.viewSelect(paramInfo, false, cssclass);
        var html = this.viewParameterGeneral(paramInfo, htmlControl);
        return html;
    };

    this.viewSelect = function(paramInfo, ismultiple, cssclass) {
        var selectedValues = null;
        var self = this;
        if (paramInfo.ParameterChoice)
            selectedValues = paramInfo.ParameterChoice.Values;
        var htmlControl = "<select id=\"" + paramInfo.id + "_control\" class=\"" + cssclass + "\" size=\"1\" ";
        if (ismultiple)
            htmlControl += " multiple=\"multiple\" ";
        htmlControl += ">";
        $.each(paramInfo.ValidValues, function(index, param) {
            
            var maybeSelected = "";
            if (selectedValues && $.inArray(param.Value, selectedValues) > -1) {
                self.addLog("param " + paramInfo.id + " value " + param.Value + " is selected (" + selectedValues + ") isArray: " + $.isArray(selectedValues));
                maybeSelected = "selected = \"selected\"";
            }
            htmlControl += "<option " + maybeSelected + " value=\"" + self.htmlEscape(param.Value) + "\">" + self.htmlEscape(param.Label) + "</option>";
        
        });
        htmlControl += "</select>";
        return htmlControl;
    };

    this.viewDatePick = function(paramInfo) {
        
        var htmlControl = "<input id=\"" + paramInfo.id + "_control\" type=\"text\" />";
        var html = this.viewParameterGeneral(paramInfo, htmlControl);
        return html;
    };

    this.viewCheckBox = function(paramInfo) {
        var maybeChecked = "";
        if (paramInfo.ParameterChoice && paramInfo.ParameterChoice.SingleValue == "True")
            maybeChecked = "checked=\"checked\"";
            
        var htmlControl = "<input id=\"" + paramInfo.id + "_control\" type=\"checkbox\" " + maybeChecked + " />";
        var html = this.viewParameterGeneral(paramInfo, htmlControl);
        return html;
    }

    this.viewTextBox = function(paramInfo, cssclass) {
        var classbit = "";
        if (cssclass)
            classbit = " class=\"" + cssclass + "\" ";
        var htmlControl = "<input id=\"" + paramInfo.id + "_control\" type=\"text\" " + classbit + "/>";
        var html = this.viewParameterGeneral(paramInfo, htmlControl);
        return html;
    }

    this.viewMultiPickLarge = function(paramInfo) {
        var htmlControl = "<div><input id=\"" + paramInfo.id + "_control\" type=\"text\" class=\"multiPickLarge\" /></div>"
            + "<div class=\"multiPickSubtitle\">Type in box to select items or <a id=\"" + paramInfo.id + "_dialogLink\" href=\"#\">click here</a> for full list</div>";
        var html = this.viewParameterGeneral(paramInfo, htmlControl);
        return html;    
    }

    this.gatherParameters = function() {
        this.addLog("gatherParameters() called");
        var self = this;
        // use an object with val property so it can be passed byref
        var paramStringObject = new Object();
        paramStringObject.val = "";
        $(".paramBlock").each(function(index) {
            var pname = this.crissCrossName;
            var dname = this.crissCrossDisplayName;
            var pval = "(unknown)";
            if (this.crissCrossValue)
                pval = this.crissCrossValue();
            self.addToParamString(paramStringObject, pname, pval);
            self.addLog("Index: " + index + " paramName: " + pname + " displayName: " + dname + " pval: " + pval);
        });
        self.addLog("paramString: " + paramStringObject.val);
        $(".hiddenParamString").val(paramStringObject.val);
        // get viewport size so we can pass it to server
        var viewportWidth = $(window).width();
        var viewportHeight = $(window).height();
        $(".hiddenViewportWidth").val(viewportWidth);
        $(".hiddenViewportHeight").val(viewportHeight);
        return true;
    };

    // adds param to paramStringObject.val in ssrs format
    this.addToParamString = function(paramStringObject, pname, pval) {
        if (!paramStringObject.val)
            paramStringObject.val = "";
        if (!$.isArray(pval)) {
            if (paramStringObject.val.length > 0)
                paramStringObject.val += "&";
            if (pval)
                paramStringObject.val += pname + "=" + this.uriEscape(pval);
            else
                paramStringObject.val += pname + ":isnull=true";
        }
        else {
            var count = 0;
            var self = this;
            $.each(pval, function(index, valloop) {
                if (paramStringObject.val.length > 0)
                    paramStringObject.val += "&";
                paramStringObject.val += pname + "=" + self.uriEscape(valloop);
                count += 1;
            });
        }
    }

    this.htmlEscape = function(str) {
        // also escapes quotes so is ok for attribute values
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    };

    this.uriEscape = function(str) {
        return encodeURIComponent(str);
    };

    this.testPageMethod = function() {
        var self = this;
        var d = {
            url: "Report.aspx/testPageMethod",
            success: function(res) {
                self.addLog("page method returned " + res.d);
            }
        };
        this.callAspNetPageMethod(d);
    };

    this.browserCheck = function() {
        var ie6orEarlier = (jQuery.browser.msie && jQuery.browser.version < 7);
        if (ie6orEarlier) {
            $("<div><p>You are using Internet Explorer 6, so CrissCross might not work properly.</p><p>Please contact your system administrator and ask to upgrade to a newer browser.</p></div>").dialog({
                height: 200,
                modal: true,
                buttons: {
                    OK: function() {
                        $(this).dialog("close");
                    }
                },
                title: "Browser Warning"
            });
        }
        // check for ie7 executable (ie check if its ie8 in compatibility mode)
        var ie7 = (jQuery.browser.msie && jQuery.browser.version < 8 && jQuery.browser.version > 6);
        var ie8 = (jQuery.browser.msie && jQuery.browser.version < 9 && jQuery.browser.version > 7);

        // made sure its not ie8 in compatibility mode
        if (ie7) {
            var ie8trident = (navigator.userAgent.indexOf("Trident/4.0") != -1);
            if (ie8trident) {
                this.addLog("browser seems to be IE8 in compatibility mode");
                ie7 = false;
                ie8 = true;
            }
        }
        this.ie7Hacks = ie6orEarlier || ie7;
        if (this.browserAdaptMode == 0) {
            this.slowBrowser = ie6orEarlier || ie7 || ie8;
        } else if (this.browserAdaptMode == 1) {
            this.slowBrowser = true;
        } else if (this.browserAdaptMode == 2) {
            this.slowBrowser = false;
        }

        this.addLog("browserCheck: ie6orEarlier: " + ie6orEarlier + " ie7: " + ie7 + " ie8: " + ie8);
        this.addLog("browserCheck: slowBrowser: " + this.slowBrowser + " browserAdaptMode: " + this.browserAdaptMode);

    };

    // call with callData.url, callData.data and callData.success
    this.callAspNetPageMethod = function(callData) {
        if (!callData.data)
            callData.data = "{}";
        var self = this;
        $.ajax({
            type: "POST",
            url: callData.url,
            contentType: "application/json; charset=utf-8",
            data: callData.data,
            dataType: "json",
            success: callData.success,
            error: function(res) {
                self.errorHandle(res.status + ' ' + res.statusText);
            }
        });
    };

    this.addLog = function(msg, isError) {
        if (!this.showLog)
            return;
        var now = new Date();
        if (!isError)
            isError = false;
        var newlog = { 'msg': msg, 'isError': isError, 'timeStamp': now.toString() };

        this.log.push(newlog);
        $(this.showLog).prepend('<div>' + newlog.timeStamp.substr(11, 8) + ' - ' + newlog.msg + '</div>');
        
    };

    this.errorHandle = function(msg, url, line) {
        var text = "Error: " + msg + " url " + url + " line " + line;
        this.addLog(text, true);
    };

    this.parseJsonDate = function(jsonDate) {
        return new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
    };


}