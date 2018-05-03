/**
 * The reason these aren't static is that the buffer cannot be resized.
 * Buffers need to know the exact size before creation.
 *
 * We essentially make a list of Message Data types, then make the
 * buffer and copy the values in.
 *
 */
import {Utility} from "../Utility";

export abstract class MessageData {
    abstract addToByteData(byteData: DataView, loc: number): void;

    abstract getLength(): number;
}

export class MessageDataUint8 implements MessageData {
    value: number;

    constructor(value: number) {
        this.value = value;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setUint8(loc, this.value);
    }

    getLength() {
        return 1;
    }
}

export class MessageDataInt8 implements MessageData {
    value: number;

    constructor(value: number) {
        this.value = value;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setInt8(loc, this.value);
    }

    getLength() {
        return 1;
    }
}

export class MessageDataUint16 implements MessageData {
    value: number;

    constructor(value: number) {
        this.value = value;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setUint16(loc, this.value, false);
    }

    getLength() {
        return 2;
    }
}

export class MessageDataInt16 implements MessageData {
    value: number;

    constructor(value: number) {
        this.value = value;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setInt16(loc, this.value, false);
    }

    getLength() {
        return 2;
    }
}

export class MessageDataUint32 implements MessageData {
    value: number;

    constructor(value: number) {
        this.value = value;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setUint32(loc, this.value, false);
    }

    getLength() {
        return 4;
    }
}

export class MessageDataInt32 implements MessageData {
    value: number;

    constructor(value: number) {
        this.value = value;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setInt32(loc, this.value, false);
    }

    getLength() {
        return 4;
    }
}

export class MessageDataFloat32 implements MessageData {
    value: number;

    constructor(value: number) {
        this.value = value;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setFloat32(loc, this.value, false);
    }

    getLength() {
        return 4;
    }
}

export class MessageDataFloat64 implements MessageData {
    value: number;

    constructor(value: number) {
        this.value = value;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setFloat64(loc, this.value, false);
    }

    getLength() {
        return 8;
    }
}

export class MessageDataString implements MessageData {
    value: ArrayBuffer;
    totalLength: number;

    constructor(value: string) {
        this.value = Utility.StringToArrayBuffer(value);
        //Total length is buffer plus length of buffer
        this.totalLength = 4 + this.value.byteLength;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setUint32(loc, this.totalLength, false);
        //Copy each byte of the value to the byteData
        let stringData = new DataView(this.value);
        let byteIndex = loc + 4;
        for (let index = 0; index < stringData.byteLength; index++) {
            byteData.setUint8(byteIndex, stringData.getUint8(index));
            byteIndex++;
        }
    }

    getLength() {
        return this.totalLength;
    }
}

export class MessageDataBinary implements MessageData {
    value: ArrayBuffer;
    totalLength: number;

    constructor(value: ArrayBuffer) {
        this.value = value;
        //Total length is buffer plus length of buffer
        this.totalLength = 4 + this.value.byteLength;
    }

    addToByteData(byteData: DataView, loc: number) {
        byteData.setUint32(loc, this.totalLength, false);
        //Copy each byte of the value to the byteData
        let data = new DataView(this.value);
        let byteIndex = loc + 4;
        for (let index = 0; index < data.byteLength; index++) {
            byteData.setUint8(byteIndex, data.getUint8(index));
            byteIndex++;
        }
    }

    getLength() {
        return this.totalLength;
    }
}