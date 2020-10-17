import React from "react";
import ReactDOM from "react-dom";
import {
  ResponsiveContainer,
  LineChart,
  Line,
  XAxis,
  YAxis,
  ReferenceLine,
  ReferenceArea,
  ReferenceDot,
  Tooltip,
  CartesianGrid,
  Legend,
  Brush,
  ErrorBar,
  AreaChart,
  Area,
  Label,
  LabelList,
} from "recharts";
import { AnalyzerApi } from "./analyzer-api";
import "./index.css";
import { getWebApiSample } from "./webapi-sample";
import { getFuncAppSample } from "./funcapp-sample";

class Game extends React.Component {
  platformId = ".net-core-3.1-azAppService";
  analyzerApi = new AnalyzerApi();

  constructor(props) {
    super(props);

    this.state = {
      id: String,
      platformId: String,
      results1: [],
      results2: [],
    };
  }

  componentDidMount() {
    // this.analyzerApi
    //   .get(".net-core-3.1-funcApp", "4cf78663-33d6-49df-bcf7-489c7b907c68")
    //   .subscribe((res) => this.setState({ results1: res }));
    // this.analyzerApi
    //   .get(".net-core-3.1-azAppService", "c52745cb-a174-4b46-b299-84aeb236a230")
    //   .subscribe((res) => this.setState({ results2: res }));

    this.setState({
      results1: getFuncAppSample().results.sort((a, b) => a.requestId - b.requestId),
      results2: getWebApiSample().results.sort((a, b) => a.requestId - b.requestId),
    });
  }

  render() {
    const { platformId, results1, results2 } = this.state;
    const data = results1.map((r, i) => {
      return {
        name: r.requestId,
        set1: r.timeTakenMs,
        set2: results2[i] != null ? results2[i].timeTakenMs : null,
      };
    });

    return (
      <div>
        <LineChart
          width={1400}
          height={400}
          data={data}
          margin={{ top: 5, right: 20, left: 10, bottom: 5 }}
        >
          <XAxis dataKey="name" />
          <Tooltip />
          <CartesianGrid stroke="#f5f5f5" />
          <Line
            type="monotone"
            dataKey="platformId"
            stroke="#ff7300"
            yAxisId={0}
          />
          <Line type="monotone" dataKey="set1" stroke="#ff7300" yAxisId={1} />
          <Line dataKey="set2" stroke="#387908" strokeWidth={2} yAxisId={1} />
        </LineChart>
      </div>
    );
  }
}

// ========================================

ReactDOM.render(<Game />, document.getElementById("root"));
