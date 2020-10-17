import { fromFetch } from "rxjs/fetch";
import { map, tap, switchMap } from "rxjs/operators";

export class AnalyzerApi {
  url =
    "http://perfanalyzer.australiaeast.azurecontainer.io/api/Analysis?platformId={platformId}&analysisId={analysisId}";

  get(platformId, analysisId) {
    return fromFetch(
      this.url
        .replace("{platformId}", platformId)
        .replace("{analysisId}", analysisId)
    ).pipe(
      switchMap((res) => res.json()),
      map((res) => res.results.sort((a, b) => a.requestId - b.requestId))
    );
  }
}
