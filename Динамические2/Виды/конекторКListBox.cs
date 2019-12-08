using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Динамические;

namespace Виды
{
    public class ГруппаОбъектовКонектор : ICollection<РАМОбъект>, INotifyCollectionChanged
    {
        public ГруппаОбъектов Группа;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public bool Remove(РАМОбъект item)
        {
            return Группа.Список.Remove(item);
        }

        public int Count => Группа.Список.Count;

        public bool IsReadOnly => false;

        public void Add(РАМОбъект объект)
        {
            Группа.Добавить(объект);
            var событие = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems: new[] { объект },
                startingIndex: Группа.Список.Count - 1);
            if (CollectionChanged != null) CollectionChanged(this, событие);
        }

        public void Clear() => Группа.Список.Clear();

        public bool Contains(РАМОбъект объект) => Группа.Список.Contains(объект);

        public void CopyTo(РАМОбъект[] array, int arrayIndex)
        {
            Группа.Список.CopyTo(array, arrayIndex);
        }


        IEnumerator<РАМОбъект> IEnumerable<РАМОбъект>.GetEnumerator()
        {
            return new НумераторГруппы(Группа);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ГруппаНумератор(Группа);
        }
    }

    public class НумераторГруппы : IEnumerator<РАМОбъект>
    {
        public ГруппаОбъектов Группа;
        int position = -1;

        public НумераторГруппы(ГруппаОбъектов группа)
        {
            Группа = группа;
        }

        public РАМОбъект Current => Группа[position];

        object IEnumerator.Current => Группа[position];

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            position++;
            return (position < Группа.Список.Count);
        }

        public void Reset()
        {
            position = -1;
        }
    }
    public class ГруппаНумератор : IEnumerator
    {
        public ГруппаОбъектов Группа;
        int position = -1;
        public ГруппаНумератор(ГруппаОбъектов группа)
        {
            Группа = группа;
        }

        public object Current => Группа[position];

        public bool MoveNext()
        {
            position++;
            return (position < Группа.Список.Count);
        }

        public void Reset()
        {
            position = -1;
        }
    }

}
