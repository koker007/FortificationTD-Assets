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


    //�������� �����
    public void AddChain(TypeData newData) {
        if (newData != null) {
            //������� �����
            Chain<TypeData> newChain = new Chain<TypeData>();
            newChain.data = newData;
            newChain.next = null;

            if (start == null) start = newChain;

            //������� ��������� ���������� ����� �� �����
            if (end != null) {
                end.next = newChain;
            }

            //����� ����� ���������� ���������
            end = newChain;
        }
    }
}
