// Copyright (c) Microsoft. All rights reserved.

import { Body1, Button, Input, Label, Spinner, Tab, TabList, Title3 } from '@fluentui/react-components';
import { FC, useEffect, useState } from 'react';
import { useSemanticKernel } from '../hooks/useSemanticKernel';
import { IKeyConfig } from '../model/KeyConfig';

interface IData {
    uri: string;
    onConfigComplete: (keyConfig: IKeyConfig) => void;
}

const ServiceConfig: FC<IData> = ({ uri, onConfigComplete }) => {
    const [isOpenAI, setIsOpenAI] = useState<boolean>(true);
    const [keyConfig, setKeyConfig] = useState<IKeyConfig>({} as IKeyConfig);
    const [isBusy, setIsBusy] = useState<boolean>(false);
    const sk = useSemanticKernel(process.env.REACT_APP_FUNCTION_URI as string);

    const [openAiKey, setOpenAiKey] = useState<string>(process.env.REACT_APP_OPEN_AI_KEY as string);
    const [openAiModel, setOpenAiModel] = useState<string>(process.env.REACT_APP_OPEN_AI_MODEL as string);
    const [azureOpenAiKey, setAzureOpenAiKey] = useState<string>(process.env.REACT_APP_AZURE_OPEN_AI_KEY as string);
    const [azureOpenAiDeployment, setAzureOpenAiDeployment] = useState<string>(
        process.env.REACT_APP_AZURE_OPEN_AI_DEPLOYMENT as string,
    );
    const [azureOpenAiEndpoint, setAzureOpenAiEndpoint] = useState<string>(
        process.env.REACT_APP_AZURE_OPEN_AI_ENDPOINT as string,
    );

    const saveKey = async () => {
        setIsBusy(true);

        //POST a simple ask to validate the key
        const ask = { value: 'clippy', inputs: [{ key: 'style', value: 'Bill & Ted' }] };

        try {
            var result = await sk.invokeAsync(keyConfig, ask, 'funskill', 'joke');
            console.log(result);
            onConfigComplete(keyConfig);
        } catch (e) {
            alert('Something went wrong.\n\nDetails:\n' + e);
        }

        setIsBusy(false);
    };

    useEffect(() => {
        keyConfig.completionConfig = {
            key: isOpenAI ? openAiKey : azureOpenAiKey,
            deploymentOrModelId: isOpenAI ? openAiModel : azureOpenAiDeployment,
            label: isOpenAI ? openAiModel : azureOpenAiDeployment,
            endpoint: isOpenAI ? '' : azureOpenAiEndpoint,
            backend: isOpenAI ? 1 : 0
        }

        setKeyConfig((keyConfig) => ({ ...keyConfig }));
    }, [isOpenAI, openAiKey, openAiModel, azureOpenAiKey, azureOpenAiDeployment, azureOpenAiEndpoint]);

    return (
        <>
            <Title3>Enter in your OpenAI or Azure OpenAI Service Key</Title3>
            <Body1>
                Start by entering in your OpenAI key, either from{' '}
                <a href="https://beta.openai.com/account/api-keys" target="_blank" rel="noreferrer">
                    OpenAI
                </a>{' '}
                or{' '}
                <a href="https://oai.azure.com/portal" target="_blank" rel="noreferrer">
                    Azure OpenAI Service
                </a>
            </Body1>

            <TabList defaultSelectedValue="oai" onTabSelect={(t, v) => setIsOpenAI(v.value === 'oai')}>
                <Tab value="oai">OpenAI</Tab>
                <Tab value="aoai">Azure OpenAI</Tab>
            </TabList>

            {isOpenAI ? (
                <>
                    <Label htmlFor="openaikey">OpenAI Key</Label>
                    <Input
                        id="openaikey"
                        type="password"
                        value={openAiKey}
                        onChange={(e, d) => {
                            setOpenAiKey(d.