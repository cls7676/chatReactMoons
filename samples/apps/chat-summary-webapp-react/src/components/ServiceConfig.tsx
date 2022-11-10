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
    const [azureOpenAiEndpoint, setAzureOpe