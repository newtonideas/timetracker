import React from 'react'
import { fetchTasks } from '../actions/tasks'
import TasksList from '../components/TasksList'
import Filters from '../components/Filters'


class TasksPage extends React.Component {
    constructor(props) {
        super(props)
    }

    componentDidMount() {
        const { dispatch } = this.props
        dispatch(fetchTasks())
    }

    render() {
        const { tasks, isFetching} = this.props
        return (
            <div className="container">

                <div className="underhead_filters">
                    <Filters/>
                </div>
                <div>
                    <TasksList tasks = {tasks} />
                </div>
            </div>
        )
    }


}