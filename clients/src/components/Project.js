import React, { Component } from 'react'
import styles from '../resources/styles'
//the loading screen for the project

class Project extends Component {
    render() {
        const {project} = this.props
        return (
            <div>
                <h3>{project.title}</h3>
            </div>
        )
    }
}

export default Project