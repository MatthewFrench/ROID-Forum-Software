'use strict';
//Pulled from https://github.com/mgechev/javascript-algorithms/tree/master/src

/**
 * Compares to elements. Default.
 * @param a
 * @param b
 * @returns {number}
 */
function compare(a : any, b : any) {
    return a - b;
}

/**
 * Quicksort algorithm
 *
 * @public
 * @param {array} array Array which should be sorted.
 * @return {array} Sorted array.
 */
export class QuickSort {
    /**
     * Quicksort algorithm. In this version of quicksort used
     * middle element of array for the pivot.<br><br>
     * Time complexity: O(N log(N)).
     *
     * @example
     *
     * let sort = require('path-to-algorithms/src' +
     * '/sorting/quicksort-middle').quickSort;
     * console.log(sort([2, 5, 1, 0, 4])); // [ 0, 1, 2, 4, 5 ]
     *
     * @public
     * @module sorting/quicksort-middle
     * @param {Array} array Input array.
     * @param {function} cmp Optional. A function that defines an
     * alternative sort order. The function should return a negative,
     * zero, or positive value, depending on the arguments.
     * @return {Array} Sorted array.
     */
    static Sort(array : any[], cmp : (a: any, b: any)=>{}) {
        cmp = cmp || compare;
        QuickSort.Quicksort(array, 0, array.length - 1, cmp);
        return array;
    };

    /**
     * Partitions the array in two parts by the middle elements.
     * All elemnts which are less than the chosen one goes left from it
     * all which are greater goes right from it.
     * Uses Hoare's partitioning algorithm.
     *
     * @param {array} array Array which should be partitioned
     * @param {number} left Left part of the array
     * @param {number} right Right part of the array
     * @param {function} cmp
     * @return {number}
     */
    static Partition(array : any[], left : any, right : any, cmp : (a: any, b: any)=>{}) {
        let pivot = array[Math.floor((left + right) / 2)];
        let temp;
        while (left <= right) {
            while (cmp(array[left], pivot) < 0) {
                left += 1;
            }
            while (cmp(array[right], pivot) > 0) {
                right -= 1;
            }
            if (left <= right) {
                temp = array[left];
                array[left] = array[right];
                array[right] = temp;
                left += 1;
                right -= 1;
            }
        }
        return left;
    }

    /**
     * Recursively calls itself with different values for
     * left/right part of the array which should be processed
     *
     * @private
     * @param {array} array Array which should be processed
     * @param {number} left Left part of the array which should be processed
     * @param {number} right Right part of the array which should be processed
     * @param {function} cmp
     */
    static Quicksort(array : any[], left : any, right : any, cmp : (a: any, b: any)=>{}) {
        let mid = QuickSort.Partition(array, left, right, cmp);
        if (left < mid - 1) {
            QuickSort.Quicksort(array, left, mid - 1, cmp);
        }
        if (right > mid) {
            QuickSort.Quicksort(array, mid, right, cmp);
        }
    }
}