import React from 'react'
import { fetchTimelogs } from '../actions/timelogs'
import TimelogsList from '../components/TimelogsList'
import Filters from '../components/Filters'


class TimelogsPage extends React.Component {
    constructor(props) {
        super(props)
    }

    componentDidMount() {
        const { dispatch } = this.props
        dispatch(fetchTimelogs())
    }


    render() {
        const { timelogs, isFetching} = this.props
        return (
            <div className="container">

                <div className="underhead_filters">
                    <Filters/>
                </div>
                <div>
                    <TimelogsList timelogs = {timelogs} />
                </div>
            </div>
        )
    }


}