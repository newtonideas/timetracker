import React, { Component } from 'react'

class Timelog extends Component {
    render() {
        return (
            <View>
                {this.props.timelogs.map((timelog, i) =>
                    <Text key={i}>{timelog.id}</Text>
                )}
            </View>
        )
    }
}

export default Timelog