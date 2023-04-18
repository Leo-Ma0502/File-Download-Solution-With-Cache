using System.Collections;
using System.Collections.Generic;
using System.Drawing;

Dictionary<string, byte[]> cache = new ();

cache.Add(Convert.ToBase64String(new byte[] {1, 2, 3}), new byte[] {1, 2, 3});

var keys = cache.Keys;

byte[] indexed = cache[Convert.ToBase64String(new byte[] { 1, 2, 3 })];

Console.WriteLine(indexed);


