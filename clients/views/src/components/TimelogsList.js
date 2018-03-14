import React from 'react'
import Timelog from './Timelog'

export default function TimelogsList({timelogs}) {
    const timelogElements = timelogs.map((timelog, index) =>
        <li key = {timelog.id} className="timelog-list_li">
            <Timelog timelog = {timelog} />
        </li>)
    return (
        <ul>
            {timelogElements}
        </ul>
    )
}