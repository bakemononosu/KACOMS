const a: any = 100; // 代入できる
console.log(a * 3); // 操作もできる

// 戻り値のない関数
function doSomething(): void { }

// 戻り値を返すことがありえない関数
function throwError(): never {
    throw new Error();
}