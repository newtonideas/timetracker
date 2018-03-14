import React from 'react'
import Project from './Project'

export default function ProjectsList({projects}) {
    const projectElements = projects.map((project, index) =>
        <li key = {project.id} className="project-list_li">
            <Project project = {project} />
        </li>)
    return (
        <ul>
            {projectElements}
        </ul>
    )
}