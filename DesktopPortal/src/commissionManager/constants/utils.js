//小数四舍五入
export function keepTwoDecimalFull(num) {
    var f = parseFloat(num);
    if (isNaN(f)) {
        return;
    }
    f = Math.round(num * 100) / 100;
    return f;
}