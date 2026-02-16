$.fn.extend({
    validate: function (data) {
        const rules = (data && data.rules) ? data.rules : {};
        let errorCount = 0;

        // helper: หา element ให้ปลอดภัย
        function getEl(idOrSelector) {
            if (!idOrSelector) return $();

            // 1) ถ้าเป็น selector อยู่แล้ว
            if (idOrSelector[0] === "#") return $(idOrSelector);

            // 2) ลองหาแบบ id ก่อน
            const $byId = $("#" + idOrSelector);
            if ($byId.length) return $byId;

            // 3) ถ้าไม่เจอ id ให้ fallback ไปหา name (สำหรับ radio/checkbox group)
            return $(`[name="${idOrSelector}"]`).first();
        }

        // helper: ดึง value ให้ถูกต้องตามชนิด input
        function getValue($el) {
            if (!$el || !$el.length) return "";

            const type = ($el.attr("type") || "").toLowerCase();

            if (type === "checkbox") {
                // ถ้าเป็น checkbox ตัวเดียว: คืนค่า "true/false"
                // ถ้าเป็น checkbox group: จะเช็คด้วย isChecked ด้านล่าง
                return $el.is(":checked") ? "true" : "";
            }

            // radio จะเช็คด้วย isChecked แบบ group
            const v = $el.val();
            return (v == null) ? "" : String(v).trim();
        }

        // helper: เช็คว่ามีเลือก/ติ๊กไหม (radio/checkbox group)
        function isAnyChecked(nameOrId) {
            // ถ้าส่งมาเป็น id ก็ลองเช็คตัวเองก่อน
            const $el = getEl(nameOrId);
            if ($el.length) {
                const type = ($el.attr("type") || "").toLowerCase();
                if (type === "checkbox" || type === "radio") {
                    // ถ้าเป็นกลุ่ม (มี name) ให้เช็คทั้งกลุ่ม
                    const name = $el.attr("name");
                    if (name) return $(`input[name="${name}"]:checked`).length > 0;
                    return $el.is(":checked");
                }
            }

            // เผื่อกรณีส่งมาเป็น name โดยตรง
            return $(`input[name="${nameOrId}"]:checked`).length > 0;
        }

        // helper: เอา rule key เป็น lower และ map เป็น function
        const validators = {
            require: function ($el, rule) {
                const v = getValue($el);
                const ok = v !== "";
                if (!ok) setError($el, rule);
                else setBlank($el, rule);
                return ok;
            },

            duplicate: function ($el, rule) {
                // rule.value = true/false (true = ซ้ำ)
                const isDup = !!(rule && rule.value);
                if (isDup) setError($el, rule);
                else setBlank($el, rule);
                return !isDup;
            },

            minlength: function ($el, rule) {
                const v = getValue($el);
                const min = parseInt(rule && rule.value, 10);
                const ok = Number.isFinite(min) ? (v.length >= min) : true;

                if (!ok) setError($el, rule);
                else setBlank($el, rule);
                return ok;
            },

            maxlength: function ($el, rule) {
                const v = getValue($el);
                const max = parseInt(rule && rule.value, 10);
                const ok = Number.isFinite(max) ? (v.length <= max) : true;

                if (!ok) setError($el, rule);
                else setBlank($el, rule);
                return ok;
            },

            selected: function ($el, rule) {
                // select / dropdown: ต้องไม่ว่าง
                const v = getValue($el);
                const ok = v !== "";
                if (!ok) setError($el, rule);
                else setBlank($el, rule);
                return ok;
            },

            filter_selected: function ($el, rule) {
                // เหมือน selected (เผื่อไว้)
                const v = getValue($el);
                const ok = v !== "";
                if (!ok) setError($el, rule);
                else setBlank($el, rule);
                return ok;
            },

            checked: function ($el, rule) {
                // checkbox / checkbox group
                const key = ($el.attr("name") || $el.attr("id"));
                const ok = key ? isAnyChecked(key) : $el.is(":checked");
                if (!ok) setError($el, rule);
                else setBlank($el, rule);
                return ok;
            },

            radio: function ($el, rule) {
                // radio group
                const key = ($el.attr("name") || $el.attr("id"));
                const ok = key ? isAnyChecked(key) : false;
                if (!ok) setError($el, rule);
                else setBlank($el, rule);
                return ok;
            },

            confirm: function ($el, rule) {
                const $password = getEl(rule && rule.value); // id ของ password
                const ok = getValue($password) === getValue($el);

                if (!ok) {
                    setError($el, rule);
                    // ให้ password ติด error ด้วย (ตามโค้ดเดิม) แต่กันไม่ให้พัง
                    if ($password.length) setError($password, { type: (rule && rule.type) || "", message: "" });
                } else {
                    setBlank($el, rule);
                    if ($password.length) setBlank($password, rule);
                }
                return ok;
            }
        };

        // วนทีละ field
        $.each(rules, function (fieldId, fieldRules) {
            const $el = getEl(fieldId);

            // ถ้า element ไม่มีใน DOM ให้ข้าม (กันพัง)
            if (!$el.length) return;

            // fieldRules อาจเป็น object ของหลาย rule
            // จะเช็คทุก rule และ "หยุดเมื่อเจอ error แรก" เพื่อ UX และลด setError ซ้ำ
            let fieldHasError = false;

            $.each(fieldRules, function (ruleKey, ruleVal) {
                if (fieldHasError) return false; // break

                const key = String(ruleKey || "").toLowerCase();
                const fn = validators[key];
                if (!fn) return; // ไม่รู้จัก rule ก็ข้าม

                const ok = fn($el, ruleVal);
                if (!ok) {
                    errorCount++;
                    fieldHasError = true;
                    return false; // break
                }
            });
        });

        return errorCount === 0;
    }
});

function setError(element, val) {
    var $group = $(element).closest(".form-group");

    $group.addClass("has-error");
    $group.find("label").css("color", "black");

    // หา help-block ภายใน form-group (ไม่ใช่ next())
    $group.find(".help-block").html(val.message || "");
}

function setErrorButton(element, val) {
    if (val.type == 'button-circle') {
        $(element).closest("label.btn-file").find('button.btn').removeClass('yellow');
        $(element).closest("label.btn-file").find('button.btn').addClass('red');
        $(element).closest("label.btn-file").find('span.help-block').html(val.message);
    }
}

function setBlankErrorButton(element, val) {
    if (val.type == 'button-circle') {
        $(element).closest("label.btn-file").find('button.btn').removeClass('red');
        $(element).closest("label.btn-file").find('button.btn').addClass('yellow');
    }
}

function setBlank(element, val) {
    var $group = $(element).closest(".form-group");

    $group.removeClass("has-error");
    $group.find(".help-block").html("");
}