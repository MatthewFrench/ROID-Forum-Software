import {MessageData, MessageDataUint16, MessageDataString, MessageDataInt32,
    MessageDataInt16, MessageDataInt8, MessageDataFloat32,
    MessageDataFloat64, MessageDataBinary, MessageDataUint8,
    MessageDataUint32} from './MessageData';

/**
 * Writes a message and produces a binary buffer.
 * Format:
 * length (4 bytes) + binary data (any size)
 */

/*
  This is currently very slow. Use ideas from:
  https://github.com/brianc/node-buffer-writer/blob/master/index.js
*/

export class MessageWriter {
  dataArray : MessageData[];
  innerByteLength : number;
  constructor() {
    this.dataArray = [];
    //Only the length of bytes being stored
    this.innerByteLength = 0;
  }

  toBuffer() : ArrayBuffer {
    //Take length of all data and add the message length holder
    let totalLength = this.innerByteLength;
    let byteData = new DataView(new ArrayBuffer(totalLength));
    let loc = 0;
    //Append the message
    for (let data of this.dataArray) {
      data.addToByteData(byteData, loc);
      loc += data.getLength();
    }
    return byteData.buffer;
  }

  addUint8(value : number) {
    let data = new MessageDataUint8(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addInt8(value : number) {
    let data = new MessageDataInt8(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addUint16(value : number) {
    let data = new MessageDataUint16(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addInt16(value : number) {
    let data = new MessageDataInt16(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addUint32(value : number) {
    let data = new MessageDataUint32(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addInt32(value : number) {
    let data = new MessageDataInt32(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addFloat64(value : number) {
    let data = new MessageDataFloat64(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addFloat32(value : number) {
    let data = new MessageDataFloat32(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addString(value : string) {
    let data = new MessageDataString(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  addBinary(value : ArrayBuffer) {
    let data = new MessageDataBinary(value);
    this.dataArray.push(data);
    this.innerByteLength += data.getLength();
  }

  getLength() : number {
    return this.innerByteLength + 4;
  }
}