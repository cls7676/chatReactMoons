// Copyright (c) Microsoft. All rights reserved.

import { IAsk } from '../model/Ask';
import { IAskResult } from '../model/AskResult';
import {
    IKeyConfig, SK_HTTP_HEADER_COMPLETION_BACKEND, SK_HTTP_HEADER_COMPLETION_ENDPOINT, SK_HTTP_HEADER_COMPLETION_KEY, SK_HTTP_HEADER_COMPLETION_MODEL, SK_HTTP_HEADER_EMBEDDING_BACKEND, SK_HTTP_HEADER_EMBEDDING_ENDPOINT, SK_HTTP_HEADER_EMBEDDING_KEY, SK_HTTP_HEADER_EMBEDDING_MODEL, SK_HTTP_HEADER_MSGRAPH
} from '../model/KeyConfig';

interface ServiceRequest {
    commandPath: string;
    method?: string;
    body?: unknown;
    keyConfig: IKeyConfig;
}

export class SemanticKernel {
    // eslint-disable-next-line @typescript-eslint/space-before-function-paren
    constructor(private readonly serviceUrl: string) { }

    public invokeAsync = async (
        keyConfig: IKeyConfig,
        ask: IAsk,
        skillName: string,
        functionName: string,
    ): Promise<IAskResult> => {
        const result = await this.getResponseAsync<IAskResult>({
            commandPath: `/api/skills/${skillName}/invoke/${functionName}`,
            method: 'POST',
            body: ask,
            keyConfig: keyConfig,
        });
        return result;
    };

    public executePlanAsync = async (keyConfig: IKeyConfig, ask: IAsk, maxSteps: number = 10): Promise<IAskResult> => {
        const result = await this.getResponseAsync<IAskResult>({
            commandPath: `/api/planner/execute/${maxSteps}`,
            method: 'POST',
            body: ask,
            keyConfig: keyConfig,
        });
        return result;
    };

    private readonly getResponseAsync = async <T>(request: ServiceRequest): Promise<T> => {
        const { commandPath, method, body, keyConfig } = request;

        const headers = new Headers();

        if (keyConfig.completionConfig !== undefined) {
            headers.append(SK_HTTP_HEADER_COMPLETION_KEY, keyConfig.completionConfig.key);
            headers.append(SK_HTTP_HEADER_COMPLETION_MODEL, keyConfig.completionConfig.deploymentOrModelId);
            headers.append(SK_HTTP_HEADER_COMPLETION_ENDPOINT, keyConfig.completionConfig.endpoint);
            headers.append(SK_HTTP_HEADER_COMPLETION_BACKEND, keyConfig.completionConfig.backend.toString());
        }

        if (keyConfig.embeddingConfig !== undefined) {
            headers.a