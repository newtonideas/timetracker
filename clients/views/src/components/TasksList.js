import React from 'react'
import Task from './Task'

export default function TasksList({tasks}) {
    const taskElements = tasks.map((task, index) =>
        <li key = {task.id} className="task-list_li">
            <Task task = {task} />
        </li>)
    return (
        <ul>
            {taskElements}
        </ul>
    )
}