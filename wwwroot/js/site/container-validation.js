function normalizeContainerNo(input) {
    if (!input) return '';
    return input.trim().toUpperCase().replace(/[\s-]/g, '');
}

function getLetterValue(ch) {
    const map = {
        A: 10, B: 12, C: 13, D: 14, E: 15,
        F: 16, G: 17, H: 18, I: 19, J: 20,
        K: 21, L: 23, M: 24, N: 25, O: 26,
        P: 27, Q: 28, R: 29, S: 30, T: 31,
        U: 32, V: 34, W: 35, X: 36, Y: 37,
        Z: 38
    };
    return map[ch] ?? -1;
}

function calculateCheckDigit(baseCode) {
    let sum = 0;

    for (let i = 0; i < baseCode.length; i++) {
        const ch = baseCode[i];
        let value;

        if (/[A-Z]/.test(ch)) {
            value = getLetterValue(ch);
            if (value === -1) {
                throw new Error('Invalid letter in container number');
            }
        } else {
            value = parseInt(ch, 10);
        }

        sum += value * Math.pow(2, i);
    }

    const remainder = sum % 11;
    return remainder === 10 ? 0 : remainder;
}

function validateContainerNo(containerNo) {
    const normalized = normalizeContainerNo(containerNo);

    if (!normalized) {
        return {
            valid: false,
            msg: 'กรุณาพิมพ์หมายเลขตู้'
        };
    }

    if (normalized.length !== 11) {
        return {
            valid: false,
            msg: 'หมายเลขตู้ต้องมี 11 ตัว'
        };
    }

    if (!/^[A-Z]{4}[0-9]{7}$/.test(normalized)) {
        return {
            valid: false,
            msg: 'รูปแบบต้องเป็น 4 ตัวอักษร ตามด้วย 7 ตัวเลข'
        };
    }

    const baseCode = normalized.substring(0, 10);
    const actual = parseInt(normalized.substring(10, 11), 10);
    const expected = calculateCheckDigit(baseCode);

    if (actual !== expected) {
        return {
            valid: false,
            msg: 'รูปแบบของหมายเลขตู้ไม่ถูกต้อง'
        };
    }

    return {
        valid: true,
        msg: '',
        normalized: normalized
    };
}

function showContainerValidation(result) {
    const $input = $('#ContainerNo');

    var $group = $input.closest(".form-group");

    if (!result.valid) {

        $group.addClass("has-error");
        $group.find("label").css("color", "black");

        // หา help-block ภายใน form-group (ไม่ใช่ next())
        $group.find(".help-block").html(result.msg || "");
    } else {
        $group.removeClass("has-error");
        $group.find(".help-block").html("");
    }
}