using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGeneric<out T> where T: Item {
    T Inner {get;}
}
