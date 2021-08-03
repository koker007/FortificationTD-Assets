using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chains<TypeData>
{
    public Chain<TypeData> start;
    Chain<TypeData> end;

    public class Chain <TypeData> {
        public Chain<TypeData> next;
        public TypeData data;
    }


    //Добавить звено
    public void AddChain(TypeData newData) {
        if (newData != null) {
            //Создаем звено
            Chain<TypeData> newChain = new Chain<TypeData>();
            newChain.data = newData;
            newChain.next = null;

            if (start == null) start = newChain;

            //Говорим ссылаться последнему звену на новое
            if (end != null) {
                end.next = newChain;
            }

            //Новое звено становится последним
            end = newChain;
        }
    }
}
