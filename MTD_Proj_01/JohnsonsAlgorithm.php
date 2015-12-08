<?php

namespace Application\Model;

/**
 * Johnsons Algorithm
 *
 * @author Kowalski Piotr
class JohnsonsAlgorithm 
 */
{
    /**
     * Algorytm wyznaczający moment rozpoczęcia i zakończenia
     * uszeregowanych zadań (wymagane do wyświetlenia wykresu)
     */
    public static function solve($data, $machinesCount)
    {          
        if ($machinesCount != 2 && $machinesCount != 3) 
            return false;
        
        $currentTick    = 0;
        $m1             = 0; 
        $m2             = 1; 
        $m3             = 2;
        $tasksCount     = count($data);
        $isThirdMachine = $machinesCount == 3;
        
        if(!$sortedTasks = self::TaskSort($data, $isThirdMachine)) 
            return false;
        
        for ($i = 0 ; $i < $tasksCount ; $i++)
        {
            $currentTask = $sortedTasks[$i];
            
            if ($i > 0) 
                $previusTask = $sortedTasks[$i - 1];

            $currentTask->Start[$m1] = $currentTick;
            $currentTick            += $currentTask->Duration[$m1];
            $currentTask->End[$m1]   = $currentTick;

            //Jeżeli czas poprzednio zakończonego zadania na M2 > od czasu aktualnie zakończonego na M1
            if ($i > 0 && $previusTask->End[$m2] > $currentTask->End[$m1]) 
                $currentTask->Start[$m2] = $previusTask->End[$m2];
            else
                $currentTask->Start[$m2] = $currentTask->End[$m1];

            $currentTask->End[$m2] = $currentTask->Start[$m2] + $currentTask->Duration[$m2];

            if ($i == $tasksCount - 1) 
                $result['totalTime'] = $currentTask->End[$m2];
        }
        
        if ($isThirdMachine)
        {
            for ($i = 0 ; $i < $tasksCount ; $i++)
            {
                $currentTask = $sortedTasks[$i];
                
                if ($i == 0)
                    $currentTask->Start[$m3] = $currentTask->End[$m2];
                
                if ($i > 0) 
                    $previusTask = $sortedTasks[$i - 1];
                
                //Jeżeli czas poprzednio zakończonego zadania na M3 > od czasu aktualnie zakończonego na M2
                if ($i > 0 && $previusTask->End[$m3] > $currentTask->End[$m2]) 
                    $currentTask->Start[$m3] = $previusTask->End[$m3];
                else
                    $currentTask->Start[$m3] = $currentTask->End[$m2];

                $currentTask->End[$m3] = $currentTask->Start[$m3] + $currentTask->Duration[$m3];

                if ($i == $tasksCount - 1) 
                    $result['totalTime'] = $currentTask->End[$m3];
            }
        }
        
        $result['tasks'] = $sortedTasks;
        return $result;
    }
    
    /**
     * Implementacja algorytmu Johnsona,
     * kryterium uszeregowania Cmax 
     */
    private static function TaskSort($tasks, $isThirdMachine)
    {
        //lista zadań których czas wykonywania na drugiej maszynie 
        
        //jest większy lub równy czasowi wykonywania na pierwszej maszynie.
        $taskList1 = array();
        //jest mniejszy niż czas wykonywania na pierwszej maszynie.
        $taskList2 = array();
        
        foreach ($tasks as $task)
        {
            if ($isThirdMachine)
            {
                //sprowadzamy do problemu 2 maszynowego ustalając czasy zadania wg: T1' = T1+T2, T2' = T2+T3
                $d1 = $task->Duration[0] + $task->Duration[1];
                $d2 = $task->Duration[1] + $task->Duration[2];

                //pomocnicze zadanie sprowadzone do dwóch maszyn
                $toCompareTask          = new Task(array($d1, $d2));
                $toCompareTask->Number  = $task->Number;
            }
            else
                $toCompareTask = $task;

            //rozdzielanie zadań do opowiednich list
            if($toCompareTask->Duration[1] >= $toCompareTask->Duration[0])
                $taskList1[] = $toCompareTask;
            else $taskList2[] = $toCompareTask;
        }

        $sortAscending = function($task1, $task2)
        {
            return $task1->Duration[0] > $task2->Duration[0];
        };
        
        $sortDescending = function($task1, $task2)
        {
            return $task1->Duration[1] < $task2->Duration[1];
        };
        //sortowanie zadań rosnąco wg czasów trwania na pierwszej maszynie
        usort($taskList1, $sortAscending);
        //sortowanie zadań malejąco wg czasów trwania na drugiej maszynie
        usort($taskList2, $sortDescending);

        //złączenie poszeregowanych list
        $sorted = array_merge($taskList1,$taskList2);

        //jeżeli do szeregowania użyto pomocniczego zadania
        //należy zwrócić uszeregowaną listę niezmodyfikowanych zadań
        if ($isThirdMachine)
        {
            foreach($sorted as $sortedTask)
            {
                foreach($tasks as $key => $task)
                {
                    if ($sortedTask->Number == $task->Number)
                    {
                        $result[] = $task;
                        unset($tasks[$key]);
                        break;
                    }
                }
            }
            return $result;
        }

        return $sorted;
    }
}

?>
