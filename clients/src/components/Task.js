import React, { Component } from 'react'

class Task extends Component {
    render() {
        const {task} = this.props
        return (
            <div>
                {task.name}
            </div>

        )
    }
}

export default Task