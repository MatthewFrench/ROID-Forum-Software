import {Utility} from "../Utility";

export class MessageReader {
    currentLoc : number;
    byteData : DataView;
    byteLength : number;
    constructor(messageData : ArrayBuffer) {
        this.byteData = new DataView(messageData);
        this.currentLoc = 0;
        this.byteLength = this.byteData.byteLength;
    }

    isAtEndOfData() : boolean {
        return this.byteLength === this.currentLoc;
    }

    hasUint8() : boolean {
        return (this.currentLoc + 1) <= this.byteLength;
    }

    getUint8() : number {
        let data = this.byteData.getUint8(this.currentLoc);
        this.currentLoc += 1;
        return data;
    }

    hasInt8() : boolean {
        return (this.currentLoc + 1) <= this.byteLength;
    }

    getInt8() : number {
        let data = this.byteData.getInt8(this.currentLoc);
        this.currentLoc += 1;
        return data;
    }

    hasUint16() : boolean {
        return (this.currentLoc + 2) <= this.byteLength;
    }

    getUint16() : number {
        let data = this.byteData.getUint16(this.currentLoc, false);
        this.currentLoc += 2;
        return data;
    }

    hasInt16() : boolean {
        return (this.currentLoc + 2) <= this.byteLength;
    }

    getInt16() : number {
        let data = this.byteData.getInt16(this.currentLoc, false);
        this.currentLoc += 2;
        return data;
    }

    hasUint32() : boolean {
        return (this.currentLoc + 4) <= this.byteLength;
    }

    getUint32() : number {
        let data = this.byteData.getUint32(this.currentLoc, false);
        this.currentLoc += 4;
        return data;
    }

    hasInt32() : boolean {
        return (this.currentLoc + 4) <= this.byteLength;
    }

    getInt32() : number {
        let data = this.byteData.getInt32(this.currentLoc, false);
        this.currentLoc += 4;
        return data;
    }

    getFloat64() : number {
        let data = this.byteData.getFloat64(this.currentLoc, false);
        this.currentLoc += 8;
        return data;
    }

    hasFloat64() : boolean {
        return (this.currentLoc + 8) <= this.byteLength;
    }

    hasFloat32() : boolean {
        return (this.currentLoc + 4) <= this.byteLength;
    }

    getFloat32() : number {
        let data = this.byteData.getFloat32(this.currentLoc, false);
        this.currentLoc += 4;
        return data;
    }

    hasString() : boolean {
        if (this.currentLoc + 4 > this.byteLength)
        {
            return false;
        }
        let length = this.byteData.getUint32(this.currentLoc, false);
        return (this.currentLoc + 4 + length) <= this.byteLength;
    }

    getString() : string {
        let length = this.byteData.getUint32(this.currentLoc, false);
        this.currentLoc += 4;
        if (length == 0) {
            return "";
        }
        let stringBuffer = this.byteData.buffer.slice(this.currentLoc, this.currentLoc + length);
        let string = Utility.ArrayBufferToString(stringBuffer);
        this.currentLoc += length;
        return string;
    }

    hasBinary() : boolean {
        if (this.currentLoc + 4 > this.byteLength)
        {
            return false;
        }
        let length = this.byteData.getUint32(this.currentLoc, false);
        return (this.currentLoc + 4 + length) <= this.byteLength;
    }

    getBinary() : ArrayBuffer {
        let length = this.byteData.getUint32(this.currentLoc, false);
        this.currentLoc += 4;
        if (length == 0) {
            return new ArrayBuffer(0);
        }
        let buffer = this.byteData.buffer.slice(this.currentLoc, this.currentLoc + length);
        this.currentLoc += length;
        return buffer;
    }

    getLength() : number {
        return this.byteLength;
    }
}