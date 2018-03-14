import React from 'react'
import { fetchProjects } from '../actions/projects'
import ProjectsList from '../components/ProjectsList'
import Filters from '../components/Filters'


class ProjectsPage extends React.Component {
    constructor(props) {
        super(props)
    }

    componentDidMount() {
        const { dispatch } = this.props
        dispatch(fetchProjects())
    }

    render() {
        const { projects, isFetching} = this.props
        return (
            <div className="container">

                <div className="underhead_filters">
                    <Filters/>
                </div>
                <div>
                    <ProjectsList projects = {projects} />
                </div>
            </div>
        )
    }


}